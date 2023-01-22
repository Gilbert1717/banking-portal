using System.Diagnostics;
using McbaSystem.Data;
using McbaSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleHashing.Net;

namespace McbaSystem.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    private readonly ISimpleHash _simpleHash = new SimpleHash();

    private readonly McbaContext _context;

    public HomeController(McbaContext context) => _context = context;

    public IActionResult Index() => View();

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

    [HttpPost]
    public async Task<IActionResult> Login(string loginID, string password)
    {
        var login = await _context.Logins.FindAsync(loginID);
        if (login == null || string.IsNullOrEmpty(password) || !_simpleHash.Verify(password, login.PasswordHash))
        {
            ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
            return View("Index", new Login { LoginID = loginID });
        }

        // Login customer.
        HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
        HttpContext.Session.SetString(nameof(Customer.Name), login.Customer.Name);

        return RedirectToAction("Index", "Account");
    }

    public IActionResult Logout()
    {
        // Logout customer.
        HttpContext.Session.Clear();

        return RedirectToAction("Index", "Home");
    }
}