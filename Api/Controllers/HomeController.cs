using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

// See here for more information:
// https://docs.microsoft.com/en-au/aspnet/core/web-api/?view=aspnetcore-7.0

[ApiController]
[Route("api")]
public class HomeController : ControllerBase
{
    // GET: api
    [HttpGet]
    public string Get()
    {
        return "Welcome to use MCBA admin API :)";
    }
}