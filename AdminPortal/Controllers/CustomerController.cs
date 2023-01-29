using System.Text;
using System.Text.RegularExpressions;
using AdminPortal.Models;
using AdminPortal.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AdminPortal.Controllers;

public class CustomerController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private HttpClient Client => _clientFactory.CreateClient("api");

    public CustomerController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    public async Task<IActionResult> Index()
    {
        return View(
            new IndexViewModel
            {
                Customers = await GetCustomers(),
                Logins = await GetLogins()
            }
        );
    }

    public async Task<IActionResult> Update(int id)
    {
        return View(await GetCustomer(id));
    }

    [HttpPost]
    public async Task<IActionResult> Update(Customer customer)
    {
        EditProfileValidation(customer);
        if (!ModelState.IsValid)
        {
            return View(customer);
        }

        var content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");

        var response = Client.PutAsync($"api/customer/{customer.CustomerID}", content).Result;

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("UpdateFailed", "API failed - Please try again");
            return View(customer);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Lock(int id)
    {
        await Client.PutAsync($"api/login/{id}/lock",null);
        
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Unlock(int id)
    {
        await Client.PutAsync($"api/login/{id}/unlock",null);
        
        return RedirectToAction(nameof(Index));
    }

    private async Task<List<Customer>> GetCustomers()
    {
        var response = await Client.GetAsync("api/customer");

        if (!response.IsSuccessStatusCode)
            throw new Exception();

        // Storing the response details received from web api.
        var result = await response.Content.ReadAsStringAsync();

        // Deserializing the response received from web api and storing into a list.
        return JsonConvert.DeserializeObject<List<Customer>>(result);
    }

    private async Task<List<Login>> GetLogins()
    {
        var response = await Client.GetAsync("api/login");

        if (!response.IsSuccessStatusCode)
            throw new Exception();

        // Storing the response details received from web api.
        var result = await response.Content.ReadAsStringAsync();

        // Deserializing the response received from web api and storing into a list.
        return JsonConvert.DeserializeObject<List<Login>>(result);
    }

    private async Task<Customer> GetCustomer(int id)
    {
        var response = await Client.GetAsync($"api/customer/{id}");

        if (!response.IsSuccessStatusCode)
            throw new Exception();

        // Storing the response details received from web api.
        var result = await response.Content.ReadAsStringAsync();

        // Deserializing the response received from web api and storing into a list.
        return JsonConvert.DeserializeObject<Customer>(result);
    }

    private void EditProfileValidation(Customer customer)
    {
        if (customer.PostCode != null && !Regex.IsMatch(customer.PostCode, @"[0-9]{4}"))
        {
            ModelState.AddModelError(nameof(customer.PostCode), "Invalid postcode");
        }

        if (customer.TFN != null && !Regex.IsMatch(customer.TFN, @"[0-9]{3} [0-9]{3} [0-9]{3}"))
        {
            ModelState.AddModelError(nameof(customer.TFN), "Incorrect format - please follow XXX XXX XXX");
        }

        if (customer.Mobile != null && !Regex.IsMatch(customer.Mobile, @"04[0-9]{2} [0-9]{3} [0-9]{3}"))
        {
            ModelState.AddModelError(nameof(customer.Mobile), "Incorrect format - please follow 04XX XXX XXX");
        }
    }
}