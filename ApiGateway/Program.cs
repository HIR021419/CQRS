using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager;
using Ocelot.Provider.Consul;

var builder = WebApplication.CreateBuilder(args);

// Charger la configuration Ocelot
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Ajouter Ocelot avec cache et Consul
builder.Services
    .AddOcelot(builder.Configuration)
    .AddCacheManager(x => x.WithDictionaryHandle())
    .AddConsul();

var app = builder.Build();

// Swagger est optionnel ici
app.MapGet("/", () => "API Gateway OK");

// Activer Ocelot
await app.UseOcelot();

app.Run();