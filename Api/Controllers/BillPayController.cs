using Api.Models;
using Api.Models.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

// See here for more information:
// https://docs.microsoft.com/en-au/aspnet/core/web-api/?view=aspnetcore-7.0

[ApiController]
[Route("api/[controller]")]
public class BillPayController : ControllerBase
{
    private readonly BillPayRepository _repo;

    public BillPayController(BillPayRepository repo)
    {
        _repo = repo;
    }

    // GET: api/billPay
    [HttpGet]
    public IEnumerable<BillPay> Get()
    {
        return _repo.GetAll();
    }

    // GET api/billPay/{id}
    [HttpGet("{id}")]
    public BillPay Get(int id)
    {
        return _repo.Get(id);
    }

    // PUT api/billPay/{id}/block
    [HttpPut("{id}/block")]
    public void Block(int id)
    {
        var billPay = _repo.Get(id);
        billPay.isBlocked = true;

        _repo.Update(billPay);
    }

    // PUT api/billPay/{id}/unblock
    [HttpPut("{id}/unblock")]
    public void Unblock(int id)
    {
        var billPay = _repo.Get(id);
        billPay.isBlocked = false;

        _repo.Update(billPay);
    }
}