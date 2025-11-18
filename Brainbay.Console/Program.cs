using System.Net.Http.Json;
using Brainbay.Domain.Data;
using Brainbay.Domain.Models;
using Brainbay.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((Action<HostBuilderContext, IServiceCollection>)((ctx, services) =>
    {
        services.AddDbContext<AppDbContext>(o =>
            o.UseSqlServer("Server=localhost;Database=AMEX;User Id=sa;Password=Sai@12345;Encrypt=False;TrustServerCertificate=True;"));

        HttpClientFactoryServiceCollectionExtensions.AddHttpClient<ICharacteropsService, RickAndMortyCharacterService>(services, (Action<HttpClient>)(c =>
        {
            c.BaseAddress = new Uri("https://rickandmortyapi.com/api/");
        }));

        services.AddScoped<ICharacterRepository, CharacterRepository>();
    }))
    .Build();

using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
await db.Database.EnsureCreatedAsync();

var service = scope.ServiceProvider.GetRequiredService<ICharacteropsService>();
var repo = scope.ServiceProvider.GetRequiredService<ICharacterRepository>();

Console.WriteLine("Fetching characters from Rick and Morty API...");
var aliveCharacters = await service.GetAliveCharactersAsync();

Console.WriteLine("Clearing DB...");
await repo.ClearAsync();

Console.WriteLine($"Saving {aliveCharacters.Count} alive characters into DB...");
await repo.SaveAsync(aliveCharacters);

Console.WriteLine("Completed.");

