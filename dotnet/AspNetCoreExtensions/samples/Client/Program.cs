using System.Net.Http.Headers;
using Api.Models;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddHttpClient();

await using var provider = services.BuildServiceProvider();
var clientFactory = provider.GetRequiredService<IHttpClientFactory>();
using var client = clientFactory.CreateClient();
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-protobuf"));

var req = new PetPark
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
using var stream = new MemoryStream(req.ToByteArray());
using var content = new StreamContent(stream);
content.Headers.ContentType = new MediaTypeHeaderValue("application/x-protobuf");

using var response = await client.PostAsync("http://localhost:5179/home/demo", content);
await using var respStream = await response.Content.ReadAsStreamAsync();
var animal = Animal.Parser.ParseFrom(respStream);
Console.WriteLine(animal.Age);