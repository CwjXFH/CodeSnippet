using Api.Database;
using EFCoreSlowQuery;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<InfoDbContext>(opt =>
{
    opt.UseSqlServer("Server=localhost;Database=Demo;User ID=sa;Password=MSSQL20241122>;Trusted_Connection=SSPI;Integrated Security=false;Encrypt=false;TrustServerCertificate=true");
});

#pragma warning disable S125 // Sections of code should not be commented out
// builder.Services.Configure<EFCoreSlowQueryOptions>(builder.Configuration.GetSection(EFCoreSlowQueryOptions.OptionsName));
#pragma warning restore S125 // Sections of code should not be commented out

var app = builder.Build();

// Configure via configuration file
#pragma warning disable S125 // Sections of code should not be commented out
// app.UseEFCoreSlowQuery();
#pragma warning restore S125 // Sections of code should not be commented out

// Configuration via code
app.UseEFCoreSlowQuery(opt =>
{
    opt.ServiceName = "DemoApi2";
    opt.SlowQueryThresholdMilliseconds = 10;
    opt.LogLevel = LogLevel.Error;
});

app.MapControllers();

await app.RunAsync();
