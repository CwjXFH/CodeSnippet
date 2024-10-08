﻿using System.Text;
using System.Text.Json;
using EC.Protobuf.Constants;
using Google.Protobuf.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace EC.Protobuf.Formatters;

public class ProtobufJsonInputFormatter : TextInputFormatter
{
    public ProtobufJsonInputFormatter()
    {
        base.SupportedEncodings.Add(InternalEncoding.UTF8NoBOM);
        base.SupportedEncodings.Add(InternalEncoding.UTF8WithBOM);
        base.SupportedEncodings.Add(Encoding.Unicode);
        base.SupportedMediaTypes.Add(HttpContentType.Application.ProtobufJson);
    }

    public ProtobufJsonInputFormatter(IEnumerable<string> mediaTypes) : this()
    {
        foreach (var type in mediaTypes)
        {
            base.SupportedMediaTypes.Add(type);
        }
    }


    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
    {
        var httpContext = context.HttpContext;
        var (inputStream, usesTranscodingStream) = GetInputStream(httpContext, encoding);

        try
        {
            object? model;
            using var reader = new StreamReader(inputStream, encoding);
            if (ProtobufJsonFormatter.TypeMessageDescriptorMap.TryGetValue(context.ModelType.FullName!, out MessageDescriptor? messageDescriptor))
            {
                model = ProtobufJsonFormatter.JsonParser.Parse(reader, messageDescriptor);
            }
            else
            {
                model = await JsonSerializer.DeserializeAsync(inputStream, context.ModelType);
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
            return (httpContext.Request.Body, false);
        }

        var inputStream = Encoding.CreateTranscodingStream(httpContext.Request.Body, encoding, Encoding.UTF8, leaveOpen: true);
        return (inputStream, true);
    }
}
