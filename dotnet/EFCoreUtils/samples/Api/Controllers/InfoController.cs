using Api.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class InfoController(InfoDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Query()
    {
        var dbResult = await dbContext.Infos
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync();
        return new JsonResult(dbResult);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody]InfoEntity info)
    {
        dbContext.Infos.Add(info);
        await dbContext.SaveChangesAsync();
        return Ok();
    }
}
