using EC.Protobuf;
using EC.Protobuf.Formatters;

var builder = WebApplication.CreateBuilder(args);

// add protobuf formatter
builder.AddProtobufFormatters(opt =>
{
    opt.InputFormatters.Clear();
    opt.OutputFormatters.Clear();
    
    opt.InputFormatters.Add(new ProtobufInputFormatter());
    opt.InputFormatters.Add(new ProtobufJsonInputFormatter("application/json"));

    opt.OutputFormatters.Add(new ProtobufOutputFormatter());
    opt.OutputFormatters.Add(new ProtobufJsonOutputFormatter());
});
builder.Services.AddHealthChecks();

var app = builder.Build();
app.UseRouting().UseHealthChecks("/health");
app.MapControllers();
await app.RunAsync();
