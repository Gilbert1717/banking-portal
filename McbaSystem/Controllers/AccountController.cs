using McbaSystem.Data;
using McbaSystem.Filters;
using McbaSystem.Models;
using McbaSystem.Services;
using McbaSystem.Utilities;
using McbaSystem.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace McbaSystem.Controllers;

// Can add authorize attribute to controllers.
[AuthorizeCustomer]
public class AccountController : Controller
{
    private readonly McbaContext _context;

    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    private readonly AccountService _accountService;

    public AccountController(McbaContext context)
    {
        _context = context;
        _accountService = new AccountService(context);
    }

    public async Task<IActionResult> Index(string link)
    {
        return View(
            new IndexViewModel
            {
                Customer = await _context.Customers.FindAsync(CustomerID),
                Action = link
            }
        );
    }

    public async Task<IActionResult> Deposit(int id)
    {
        return View(
            new ActionViewModel
            {
                Account = await _context.Accounts.FindAsync(id)
            });
    }


    [HttpPost]
    public async Task<IActionResult> Deposit(int id, ActionViewModel model)
    {
        AmountErrorMessage(model.Transaction.Amount);
        model.Account = await _context.Accounts.FindAsync(id);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        return View("Confirm", model);
    }


    [HttpPost]
    public async Task<IActionResult> Confirm(Transaction transaction)
    {
        Account account = await _context.Accounts.Include(x => x.Transactions)
            .Where(x => x.AccountNumber == transaction.AccountNumber).FirstOrDefaultAsync<Account>();

        _accountService.HandleTransaction(transaction, account);
        switch (transaction.TransactionType)
        {
            case TransactionType.Withdraw:
                _accountService.WithdrawServiceFeeCharge(account);
                break;
            case TransactionType.Transfer:
                var destinationAccountNumber = (int)transaction.DestinationAccountNumber;
                Transaction incomingTransaction = new Transaction
                {
                    TransactionType = TransactionType.Transfer,
                    Comment = transaction.Comment,
                    AccountNumber = destinationAccountNumber,
                    Amount = transaction.Amount * -1,
                    TransactionTimeUtc = DateTime.Now.ToUniversalTime()
                };
                var destinationAccount = await _context.Accounts.FindAsync(destinationAccountNumber);
                _accountService.HandleTransaction(incomingTransaction, destinationAccount);
                _accountService.TransferServiceFeeCharge(account);
                break;
        }

        _context.Update(account);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Withdraw(int id)
    {
        return View(
            new ActionViewModel
            {
                Account = await _context.Accounts.FindAsync(id)
            });
    }

    [HttpPost]
    public async Task<IActionResult> Withdraw(int id, ActionViewModel model)
    {
        Account account = await _context.Accounts.FindAsync(id);
        AmountErrorMessage(model.Transaction.Amount);
        WithdrawAmountValidation(model.Transaction.Amount, account);

        model.Account = account;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.Transaction.Amount *= -1;

        return View("Confirm", model);
    }

    public async Task<IActionResult> Transfer(int id)
    {
        return View(
            new ActionViewModel
            {
                Account = await _context.Accounts.FindAsync(id)
            });
    }

    [HttpPost]
    public async Task<IActionResult> Transfer(int id, ActionViewModel model)
    {
        var account = await _context.Accounts.FindAsync(id);
        model.Account = account;
        AmountErrorMessage(model.Transaction.Amount);
        DestinationAccountNumberValidation(model.Transaction.DestinationAccountNumber, account);
        WithdrawAmountValidation(model.Transaction.Amount, account);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.Transaction.Amount *= -1;
        return View("Confirm", model);
    }

    private void DestinationAccountNumberValidation(int? id, Account account)
    {
        var key = "Transaction.DestinationAccountNumber";
        if (id == null)
            ModelState.AddModelError(key, "Destination account number cannot be empty");

        else if (id == account.AccountNumber)
            ModelState.AddModelError(key, "Unable to transfer money to the same account");

        else if (_context.Accounts.Find(id) == null)
            ModelState.AddModelError(key, "The account number does not exist");
    }

    private void AmountErrorMessage(decimal amount)
    {
        if (amount <= 0)
            ModelState.AddModelError("Transaction.Amount", "Amount must be positive.");

        if (amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError("Transaction.Amount", "Amount cannot have more than 2 decimal places.");
    }

    private void WithdrawAmountValidation(decimal amount, Account account)
    {
        if (account.InsufficientAmount(amount))
            ModelState.AddModelError("Transaction.Amount", "Insufficient balance");
    }
}