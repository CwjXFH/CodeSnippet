using EC.Protobuf;
using EC.Protobuf.Formatters;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
});

// Response compression (Brotli preferred, Gzip fallback)
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.SmallestSize);
builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.SmallestSize);

// add protobuf formatter
builder.AddProtobufFormatters(opt =>
{
    opt.InputFormatters.Clear();
    opt.OutputFormatters.Clear();

    opt.InputFormatters.Add(new ProtobufInputFormatter());
    opt.InputFormatters.Add(new ProtobufJsonInputFormatter(["application/json", "application/x-json"]));

    opt.OutputFormatters.Add(new ProtobufOutputFormatter());
    opt.OutputFormatters.Add(new ProtobufJsonOutputFormatter());
});
builder.Services.AddHealthChecks();

var app = builder.Build();
app.UseResponseCompression();
app.UseRouting().UseHealthChecks("/health");
app.MapControllers();
await app.RunAsync();
