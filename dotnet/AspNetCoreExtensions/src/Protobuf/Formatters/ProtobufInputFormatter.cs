using System.Text;
using Google.Protobuf.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Protobuf.Constants;

namespace Protobuf.Formatters;

internal class ProtobufInputFormatter : TextInputFormatter
{
    public ProtobufInputFormatter()
    {
        base.SupportedEncodings.Add(InternalEncoding.UTF8NoBOM);
        base.SupportedEncodings.Add(InternalEncoding.UTF8WithBOM);
        base.SupportedEncodings.Add(Encoding.Unicode);
        base.SupportedMediaTypes.Add(HttpContentType.Application.Protobuf);
    }


    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
    {
        var httpContext = context.HttpContext;
        var (inputStream, usesTranscodingStream) = GetInputStream(httpContext, encoding);

        try
        {
            object? model;
            if (ProtobufJsonFormatter.TypeMessageDescriptorMap.TryGetValue(context.ModelType.FullName!, out MessageDescriptor? messageDescriptor))
            {
                model = messageDescriptor.Parser.ParseFrom(inputStream);
            }
            else
            {
                throw new NotSupportedException($"Cannot find type info:{context.ModelType.FullName!}");
            }

            if (model == null)
            {
                return await InputFormatterResult.NoValueAsync();
            }

            return await InputFormatterResult.SuccessAsync(model);
        }
        finally
        {
            if (usesTranscodingStream)
            {
                await inputStream.DisposeAsync();
            }
        }
    }


    private static (Stream inputStream, bool usesTranscodingStream) GetInputStream(HttpContext httpContext, Encoding encoding)
    {
        if (encoding.CodePage == Encoding.UTF8.CodePage)
        {
            return (inputStream: httpContext.Request.Body, usesTranscodingStream: false);
        }

        return (inputStream: Encoding.CreateTranscodingStream(httpContext.Request.Body, encoding, Encoding.UTF8, leaveOpen: true),
            usesTranscodingStream: true);
    }
}
