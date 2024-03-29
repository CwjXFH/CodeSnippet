using EC.Protobuf;

var builder = WebApplication.CreateBuilder(args);

// add protobuf formatter
builder.AddProtobufFormatters();
builder.Services.AddHealthChecks();

var app = builder.Build();
app.UseRouting().UseHealthChecks("/health");
app.MapControllers();
app.Run();