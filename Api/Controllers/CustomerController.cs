using Api.Models;
using Api.Models.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

// See here for more information:
// https://docs.microsoft.com/en-au/aspnet/core/web-api/?view=aspnetcore-7.0

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly CustomerRepository _repo;

    public CustomerController(CustomerRepository repo)
    {
        _repo = repo;
    }

    // GET: api/customer
    [HttpGet]
    public IEnumerable<Customer> Get()
    {
        return _repo.GetAll();
    }

    // GET api/customer/{id}
    [HttpGet("{id}")]
    public Customer Get(int id)
    {
        return _repo.Get(id);
    }

    // PUT api/customer/{id}
    [HttpPut("{id}")]
    public void Put([FromBody] Customer customer)
    {
        _repo.Update(customer);
    }
}