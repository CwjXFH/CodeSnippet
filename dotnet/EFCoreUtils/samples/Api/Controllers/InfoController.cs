using Api.Database;
using EFCoreExtensions.Queryable;
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
        // var dbResult = await _dbContext.Infos
        //     .AsNoTracking()
        //     .OrderBy(x => x.Id)
        //     .ToListAsync(10, CancellationToken.None);
        // //.FirstOrDefaultAsync(e => e.Tag == tag, CancellationToken.None);
        // return new JsonResult(dbResult);
        return Content(_dbContext.Infos.NonQueryTagWith("non query tag").ToQueryString());
    }

    [HttpPost]
    public async Task<IActionResult> Create()
    {
        var info = new InfoEntity();
        var queryable = (IQueryable<InfoEntity>) _dbContext.Infos;
        queryable.TagWith("create_entity");
        // var dbset = (DbSet<InfoEntity>)_dbContext.Infos.TagWith("create_entity");
        _dbContext.Infos.Add(info);
        await using var tx = await _dbContext.Database.BeginTransactionAsync();
        await _dbContext.SaveChangesAsync();
        await tx.RollbackAsync();
        return Ok();
    }
}