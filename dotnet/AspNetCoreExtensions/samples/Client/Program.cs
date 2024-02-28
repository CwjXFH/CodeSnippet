using System.Net.Http.Headers;
using Api.Models;
using EC.Protobuf.Constants;
using EC.Protobuf.Formatters;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddHttpClient();

await using var provider = services.BuildServiceProvider();
var clientFactory = provider.GetRequiredService<IHttpClientFactory>();


var park = new PetPark
{
    Dogs =
    {
        new Animal
        {
            Name = "dog",
            Age = 3
        }
    }
};

var animal1 = await ProtobufJsonReqAsync(park);
var animal2 = await ProtobufReqAsync(park);
Console.WriteLine(animal1.Age);
Console.WriteLine(animal2.Age);

return;

async Task<Animal> ProtobufJsonReqAsync(PetPark request)
{
    using var client = clientFactory!.CreateClient();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(HttpContentType.Application.ProtobufJson));

    var jsonTxt = ProtobufJsonFormatter.JsonFormatter.Format(request);
    using var content = new StringContent(jsonTxt);
    content.Headers.ContentType = new MediaTypeHeaderValue(HttpContentType.Application.ProtobufJson);

    using var response = await client.PostAsync("http://localhost:5179/home/demo", content);
    var respTxt = await response.Content.ReadAsStringAsync();
    return ProtobufJsonFormatter.JsonParser.Parse<Animal>(respTxt);
}

async Task<Animal> ProtobufReqAsync(PetPark request)
{
    using var client = clientFactory.CreateClient();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(HttpContentType.Application.Protobuf));

    using var stream = new MemoryStream(request.ToByteArray());
    using var content = new StreamContent(stream);
    content.Headers.ContentType = new MediaTypeHeaderValue(HttpContentType.Application.Protobuf);

    using var response = await client.PostAsync("http://localhost:5179/home/demo", content);
    await using var respStream = await response.Content.ReadAsStreamAsync();
    return Animal.Parser.ParseFrom(respStream);
}