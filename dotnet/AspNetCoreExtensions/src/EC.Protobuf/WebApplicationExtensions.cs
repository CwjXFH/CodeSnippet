using EC.Protobuf.Formatters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace EC.Protobuf;

public static class WebApplicationExtensions
{
    public static void AddProtobufFormatters(this WebApplicationBuilder builder, Action<MvcOptions>? mvcOpts = null)
    {
        ProtobufJsonFormatter.RegistryMessageDescriptor();
        mvcOpts ??= opt =>
        {
            // opt.InputFormatters.Clear();
            opt.InputFormatters.Add(new ProtobufInputFormatter());
            opt.InputFormatters.Add(new ProtobufJsonInputFormatter());

            opt.OutputFormatters.Clear();
            opt.OutputFormatters.Add(new ProtobufOutputFormatter());
            opt.OutputFormatters.Add(new ProtobufJsonOutputFormatter());
        };
        builder.Services.AddControllers(mvcOpts);
    }
}