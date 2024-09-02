using System.Text;
using System.Text.Json;
using EC.Protobuf.Constants;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace EC.Protobuf.Formatters;

public class ProtobufJsonOutputFormatter : TextOutputFormatter
{
    public ProtobufJsonOutputFormatter()
    {
        base.SupportedEncodings.Add(InternalEncoding.UTF8NoBOM);
        base.SupportedEncodings.Add(InternalEncoding.UTF8WithBOM);
        base.SupportedEncodings.Add(Encoding.Unicode);
        base.SupportedMediaTypes.Add(HttpContentType.Application.ProtobufJson);
    }

    public ProtobufJsonOutputFormatter(string mediaType) : this()
    {
        base.SupportedMediaTypes.Add(mediaType);
    }


    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        var httpContext = context.HttpContext;
        var selectedUtf8 = selectedEncoding.CodePage == Encoding.UTF8.CodePage;

        if (selectedUtf8)
        {
            var responseStream = httpContext.Response.Body;

            if (context.Object is IMessage)
            {
                await using var writer = new StreamWriter(responseStream, selectedEncoding);
                ProtobufJsonFormatter.JsonFormatter.WriteValue(writer, context.Object);
            }
            else
            {
                await JsonSerializer.SerializeAsync(responseStream, context.Object);
            }

            await responseStream.FlushAsync(context.HttpContext.RequestAborted);
            return;
        }


        await using var transcodingStream =
            Encoding.CreateTranscodingStream(httpContext.Response.Body, selectedEncoding, Encoding.UTF8, leaveOpen: true);
        StreamWriter? transcodingWriter = null;
        try
        {
            if (context.Object is IMessage)
            {
                transcodingWriter = new StreamWriter(transcodingStream, selectedEncoding);
                ProtobufJsonFormatter.JsonFormatter.WriteValue(transcodingWriter, context.Object);
            }
            else
            {
                await JsonSerializer.SerializeAsync(transcodingStream, context.Object);
            }

            await transcodingStream.FlushAsync();
        }
        finally
        {
            if (transcodingWriter != null)
            {
                await transcodingWriter.DisposeAsync();
            }
        }
    }
}
