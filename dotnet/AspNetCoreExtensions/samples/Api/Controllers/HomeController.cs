using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("[controller]/[action]")]
public class HomeController : ControllerBase
{
    [HttpPost]
    public ActionResult Demo([FromBody] PetPark park)
    {
        var dog = park.Dogs.FirstOrDefault() ?? new Animal();
    
        return Ok(dog);
    }
}