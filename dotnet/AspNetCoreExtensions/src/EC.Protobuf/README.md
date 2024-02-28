Protobuf formatter for ASP.NET Core.

This library support `application/x-protobuf-json` and `application/x-protobuf` HTTP request headers:
```http request
POST your_api_url
Content-Type: application/x-protobuf-json
Accept: application/x-protobuf-json

{
  "dogs": [
    {
      "name": "d1",
      "age": 3
    }
  ]
}
```
How to use proto files in c# project： [C# Tooling support for .proto files](https://learn.microsoft.com/en-us/aspnet/core/grpc/basics?view=aspnetcore-8.0#c-tooling-support-for-proto-files).

You can get more details in the [sample projects](https://github.com/CwjXFH/CodeSnippet/tree/master/dotnet/AspNetCoreExtensions/samples).
