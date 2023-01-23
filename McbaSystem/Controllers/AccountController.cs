using McbaSystem.Data;
using McbaSystem.Filters;
using McbaSystem.Models;
using McbaSystem.Utilities;
using McbaSystem.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;

namespace McbaSystem.Controllers;

// Can add authorize attribute to controllers.
[AuthorizeCustomer]
public class AccountController : Controller
{
    private readonly McbaContext _context;


    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    private readonly MenuService _menuService;

    public AccountController(McbaContext context)
    {
        _context = context;
        _menuService = new MenuService(context);
    }

    /**
     * Default to land on Deposit page if link is not provided.
     */
    public async Task<IActionResult> Index(string link = "Deposit")
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
        Account account = await _context.Accounts.FindAsync(transaction.AccountNumber);
        _menuService.HandleTransaction(transaction, account);
        switch (transaction.TransactionType)
        {
            case TransactionType.Withdraw:
                _menuService.WithdrawServiceFeeCharge(account);
                break;
            case TransactionType.Transfer:
                _menuService.TransferServiceFeeCharge(account);
                break;
        }
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


    //Todo: Transfer
    // [HttpPost]
    // public async Task<IActionResult> Transfer(int id, AccountPageViewModel model)
    // {
    //     var account = await _context.Accounts.FindAsync(id);
    //     AmountErrorMessage(model.Transaction.Amount);
    //     WithdrawAmountValidation(model.Transaction.Amount, account);
    //     
    //
    //     if (!ModelState.IsValid)
    //     {
    //         model.Account = account;
    //         return View(model);
    //     }
    //
    //     model.Transaction.Amount *= -1;
    //     
    //
    //     return RedirectToAction("Confirm", model);
    // }

    //Todo: ScheduledBill
    [HttpPost]
    // public async Task<IActionResult> BillPay(int id, AccountPageViewModel model)
    // {
    //     var account = await _context.Accounts.FindAsync(id);
    //     AmountErrorMessage(model.Transaction.Amount);
    //     WithdrawAmountValidation(model.Transaction.Amount, account);
    //     if (!ModelState.IsValid)
    //     {
    //         ViewBag.Amount = model.Transaction.Amount;
    //         ViewBag.Comment = model.Transaction.Comment;
    //         return View("Deposit", model);
    //     }
    //
    //     _menuService.HandleTransaction(TransactionType.Withdraw, model.Transaction.Comment,
    //         model.Transaction.Amount * -1, account);
    //     _menuService.WithdrawServiceFeeCharge(account);
    //     await _context.SaveChangesAsync();
    //     return RedirectToAction(nameof(Index));
    // }
    private void AmountErrorMessage(decimal amount)
    {
        if (amount <= 0)
            ModelState.AddModelError("Transaction.Amount", "Amount must be positive.");

        if (amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError("Transaction.Amount", "Amount cannot have more than 2 decimal places.");
    }

    public void WithdrawAmountValidation(decimal amount, Account account)
    {
        if (account.InsufficientAmount(amount))
            ModelState.AddModelError("Transaction.Amount", "Insufficient balance");
    }
}