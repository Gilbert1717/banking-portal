using AdminPortal.Filters;
using AdminPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AdminPortal.Controllers;

[AuthorizeCustomer]
public class BillPayController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private HttpClient Client => _clientFactory.CreateClient("api");

    public BillPayController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    public async Task<IActionResult> Index()
    {
        return View(await GetBillPays());
    }

    public async Task<IActionResult> Block(int id)
    {
        await Client.PutAsync($"api/billPay/{id}/block", null);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> UnBlock(int id)
    {
        await Client.PutAsync($"api/billPay/{id}/unblock", null);

        return RedirectToAction(nameof(Index));
    }

    private async Task<List<BillPay>> GetBillPays()
    {
        var response = await Client.GetAsync("api/billPay");

        if (!response.IsSuccessStatusCode)
            throw new Exception();

        // Storing the response details received from web api.
        var result = await response.Content.ReadAsStringAsync();

        // Deserializing the response received from web api and storing into a list.
        return JsonConvert.DeserializeObject<List<BillPay>>(result);
    }
}