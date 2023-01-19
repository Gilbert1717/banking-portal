using McbaSystem.Data;
using McbaSystem.Filters;
using McbaSystem.Models;
using McbaSystem.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace McbaSystem.Controllers;

// Can add authorize attribute to controllers.
[AuthorizeCustomer]
public class AccountPageController : Controller
{
    private readonly McbaContext _context;


    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    private readonly MenuService _menuService;

    public AccountPageController(McbaContext context)
    {
        _context = context;
        _menuService = new MenuService(context);
    }


    //[AuthorizeCustomer]
    public async Task<IActionResult> Index()
    {
        var customer = await _context.Customers.Include(x => x.Accounts)
            .FirstOrDefaultAsync(x => x.CustomerID == CustomerID);

        return View(customer);
    }

    public async Task<IActionResult> Deposit(int id) => View(await _context.Accounts.FindAsync(id));


    [HttpPost]
    public async Task<IActionResult> Deposit(int id, decimal amount, string comment)
    {
        var account = await _context.Accounts.Include(x => x.Transactions)
            .FirstOrDefaultAsync(x => x.AccountNumber == id);
        AmountErrorMessage(amount);
        if (!ModelState.IsValid)
        {
            ViewBag.Amount = amount;
            ViewBag.Comment = comment;
            return View(account);
        }

        _menuService.HandleTransaction(TransactionType.Deposit, comment, amount, account);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    
    public async Task<IActionResult> Withdraw(int id) => View(await _context.Accounts.FindAsync(id));
    [HttpPost]
    public async Task<IActionResult> Withdraw(int id, decimal amount, string comment)
    {
        var account = await _context.Accounts.FindAsync(id);
        AmountErrorMessage(amount);
        WithdrawAmountValidation(amount, account);
        if (!ModelState.IsValid)
        {
            ViewBag.Amount = amount;
            ViewBag.Comment = comment;
            return View("Deposit", account);
        }

        _menuService.HandleTransaction(TransactionType.Withdraw, comment, amount * -1, account);
        _menuService.WithdrawServiceFeeCharge(account);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    //Todo: Transfer
    [HttpPost]
    public async Task<IActionResult> Transfer(int id, decimal amount, string comment)
    {
        var account = await _context.Accounts.FindAsync(id);
        AmountErrorMessage(amount);
        if (!ModelState.IsValid)
        {
            ViewBag.Amount = amount;
            ViewBag.Comment = comment;
            return View("Deposit", account);
        }

        _menuService.HandleTransaction(TransactionType.Deposit, comment, amount, account);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    //Todo: ScheduledBill
    [HttpPost]
    public async Task<IActionResult> ScheduledBill(int id, decimal amount, string comment)
    {
        var account = await _context.Accounts.FindAsync(id);
        AmountErrorMessage(amount);
        if (!ModelState.IsValid)
        {
            ViewBag.Amount = amount;
            ViewBag.Comment = comment;
            return View("Deposit", account);
        }

        _menuService.HandleTransaction(TransactionType.Deposit, comment, amount, account);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private void AmountErrorMessage(decimal amount)
    {
        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");

        if (amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
    }

    public void WithdrawAmountValidation(decimal amount, Account account)
    {
        if (account.InsufficientAmount(amount))
            ModelState.AddModelError(nameof(amount), "Insufficient balance");
    }
}