using Api.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class InfoController : ControllerBase
{
    private readonly InfoDbContext _dbContext;

    public InfoController(InfoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Query([FromQuery] string tag)
    {
        var dbResult = await _dbContext.Infos
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync();
        return new JsonResult(dbResult);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody]InfoEntity info)
    {
        _dbContext.Infos.Add(info);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
}