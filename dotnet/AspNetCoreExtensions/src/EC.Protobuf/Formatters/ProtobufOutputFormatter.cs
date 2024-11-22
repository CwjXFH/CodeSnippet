using System.Text;
using EC.Protobuf.Constants;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace EC.Protobuf.Formatters;

public class ProtobufOutputFormatter : TextOutputFormatter
{
    public ProtobufOutputFormatter()
    {
        base.SupportedEncodings.Add(InternalEncoding.UTF8NoBOM);
        base.SupportedEncodings.Add(InternalEncoding.UTF8WithBOM);
        base.SupportedEncodings.Add(Encoding.Unicode);
        base.SupportedMediaTypes.Add(HttpContentType.Application.Protobuf);
    }

    public ProtobufOutputFormatter(IEnumerable<string> mediaTypes) : this()
    {
        foreach (var type in mediaTypes)
        {
            base.SupportedMediaTypes.Add(type);
        }
    }


    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        var httpContext = context.HttpContext;
        var selectedUtf8 = selectedEncoding.CodePage == Encoding.UTF8.CodePage;
        var buffer = context.Object is IMessage message ? message.ToByteArray() : [];

        if (selectedUtf8)
        {
            var responseStream = httpContext.Response.Body;
            await responseStream.WriteAsync(buffer);
            await responseStream.FlushAsync(context.HttpContext.RequestAborted);
            return;
        }

        await using var transcodingStream =
            Encoding.CreateTranscodingStream(httpContext.Response.Body, selectedEncoding, Encoding.UTF8, leaveOpen: true);
        await transcodingStream.WriteAsync(buffer);
        await transcodingStream.FlushAsync();
    }
}
