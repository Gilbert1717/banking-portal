using Api.Models;
using Api.Models.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

// See here for more information:
// https://docs.microsoft.com/en-au/aspnet/core/web-api/?view=aspnetcore-7.0

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly LoginRepository _repo;

    public LoginController(LoginRepository repo)
    {
        _repo = repo;
    }

    // GET: api/login
    [HttpGet]
    public IEnumerable<Login> Get()
    {
        return _repo.GetAll();
    }

    // GET api/login/{id}
    [HttpGet("{id}")]
    public Login Get(int id)
    {
        return _repo.Get(id);
    }

    // PUT api/login/{id}/lock
    [HttpPut("{id}/lock")]
    public void Lock(int id)
    {
        var login = _repo.Get(id);
        login.isLocked = true;

        _repo.Update(login);
    }

    // PUT api/login/{id}/unlock
    [HttpPut("{id}/unlock")]
    public void Unlock(int id)
    {
        var login = _repo.Get(id);
        login.isLocked = false;

        _repo.Update(login);
    }
}