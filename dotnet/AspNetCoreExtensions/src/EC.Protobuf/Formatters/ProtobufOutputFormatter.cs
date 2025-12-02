using EC.Protobuf.Constants;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace EC.Protobuf.Formatters;

public class ProtobufOutputFormatter : IOutputFormatter
{
    private MediaTypeCollection _supportedMediaTypes { get; } = [];

    public ProtobufOutputFormatter()
    {
        _supportedMediaTypes.Add(HttpContentType.Application.Protobuf);
    }

    public ProtobufOutputFormatter(IEnumerable<string> mediaTypes) : this()
    {
        foreach (var type in mediaTypes)
        {
            _supportedMediaTypes.Add(type);
        }
    }


    public bool CanWriteResult(OutputFormatterCanWriteContext context)
    {
        var contentTypeSupported = false;
        if (context.ContentType.HasValue)
        {
            contentTypeSupported = _supportedMediaTypes.Any(mt =>
                mt.Equals(context.ContentType.Value, StringComparison.OrdinalIgnoreCase));
        }

        if (contentTypeSupported == false)
        {
            return false;
        }

        if (context.Object is not IMessage && context.Object is not Stream)
        {
            return false;
        }

        return true;
    }

    public async Task WriteAsync(OutputFormatterWriteContext context)
    {
        var response = context.HttpContext.Response;

        if (context.ContentType != null)
        {
            response.ContentType = context.ContentType.Value;
        }

        if (context.Object is IMessage message)
        {
            message.WriteTo(response.Body);
            await response.Body.FlushAsync(context.HttpContext.RequestAborted);
            return;
        }

        if (context.Object is Stream stream)
        {
            using var valueAsStream = stream;
            await valueAsStream.CopyToAsync(response.Body, context.HttpContext.RequestAborted);
        }
    }
}
