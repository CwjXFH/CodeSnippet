using EC.Protobuf.Constants;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EC.Protobuf.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class AutoSetResponseContentTypeFilterAttribute : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        var request = context.HttpContext.Request;
        
        if (request.Headers.TryGetValue("Accept", out var accept) == false || accept is not { Count: > 0 })
        {
            context.HttpContext.Response.ContentType = context.HttpContext.Request.ContentType ?? HttpContentType.Application.ProtobufJson;
            return;
        }

        foreach (var item in accept)
        {
            if (string.Equals(item, HttpContentType.Application.Protobuf, StringComparison.OrdinalIgnoreCase))
            {
                context.HttpContext.Response.ContentType = HttpContentType.Application.Protobuf;
                break;
            }
        }

        if (context.HttpContext.Response.ContentType is not { Length: > 0 })
        {
            context.HttpContext.Response.ContentType = HttpContentType.Application.ProtobufJson;
        }
    }
}