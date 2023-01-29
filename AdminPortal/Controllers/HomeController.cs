using System.Diagnostics;
using AdminPortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPortal.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    public IActionResult Index() => View();

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        if (username != "admin" || password != "admin")
        {
            ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
            return View("Index");
        }

        // Login admin
        HttpContext.Session.SetString("user", username);

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Logout()
    {
        // Logout admin
        HttpContext.Session.Clear();

        return RedirectToAction(nameof(Index));
    }
}