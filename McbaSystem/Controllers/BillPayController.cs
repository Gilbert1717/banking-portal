using McbaSystem.Data;
using McbaSystem.Filters;
using McbaSystem.Models;
using McbaSystem.Utilities;
using McbaSystem.ViewModels.BillPay;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace McbaSystem.Controllers;

// Can add authorize attribute to controllers.
[AuthorizeCustomer]
public class BillPayController : Controller
{
    private readonly McbaContext _context;

    public BillPayController(McbaContext context) => _context = context;

    public async Task<IActionResult> Index(int id)
    {
        var account = await _context.Accounts.FindAsync(id);
        return View(account);
    }

    public async Task<IActionResult> Create(int id)
    {
        return View("Form",
            new FormViewModel
            {
                Payees = await _context.Payees.ToListAsync(),
                BillPay = new BillPay
                {
                    AccountNumber = id,
                    ScheduleTimeUtc = DateTime.Today.AddDays(1)
                },
                Action = "Create"
            });
    }

    [HttpPost]
    public async Task<IActionResult> Create(FormViewModel model)
    {
        var billPay = model.BillPay;
        ScheduleTimeValidation(billPay.ScheduleTimeUtc);
        AmountValidation(billPay.Amount);
        if (!ModelState.IsValid)
        {
            return View("Form",
                new FormViewModel
                {
                    Payees = await _context.Payees.ToListAsync(),
                    BillPay = billPay,
                    Action = "Create"
                });
        }

        billPay.ScheduleTimeUtc = billPay.ScheduleTimeUtc.ToUniversalTime();
        _context.Add(billPay);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { id = billPay.AccountNumber });
    }

    public async Task<IActionResult> Cancel(int id)
    {
        var billPay = await _context.BillPays.FindAsync(id);
        var accountNumber = billPay.AccountNumber;
        _context.Remove(billPay);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { id = accountNumber });
    }

    public async Task<IActionResult> Update(int id)
    {
        var billPay = await _context.BillPays.FindAsync(id);
        billPay.ScheduleTimeUtc = billPay.ScheduleTimeUtc.ToLocalTime();

        return View("Form",
            new FormViewModel
            {
                Payees = await _context.Payees.ToListAsync(),
                BillPay = billPay,
                Action = "Update"
            });
    }

    [HttpPost]
    public async Task<IActionResult> Update(FormViewModel model)
    {
        var billPay = model.BillPay;
        ScheduleTimeValidation(billPay.ScheduleTimeUtc);
        AmountValidation(billPay.Amount);
        if (!ModelState.IsValid)
        {
            return View("Form",
                new FormViewModel
                {
                    Payees = await _context.Payees.ToListAsync(),
                    BillPay = billPay,
                    Action = "Update"
                });
        }

        billPay.ScheduleTimeUtc = billPay.ScheduleTimeUtc.ToUniversalTime();
        billPay.ErrorMessage = null;
        _context.Update(billPay);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { id = billPay.AccountNumber });
    }

    private void ScheduleTimeValidation(DateTime time)
    {
        if (time <= DateTime.Now)
            ModelState.AddModelError("BillPay.ScheduleTimeUtc", "Please schedule a future payment");
    }

    private void AmountValidation(decimal amount)
    {
        if (amount <= 0)
            ModelState.AddModelError("BillPay.Amount", "Amount must be positive.");

        if (amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError("BillPay.Amount", "Amount cannot have more than 2 decimal places.");
    }
}