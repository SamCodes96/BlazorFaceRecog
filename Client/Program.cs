using BlazorFaceRecog.Client;
using BlazorFaceRecog.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using Refit;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddTransient(sp =>
{
    var client = sp.GetService<HttpClient>()!;
    return RestService.For<IApiService>(client);
});

builder.Services.AddScoped<IHubService, HubService>();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration = new()
    {
        PositionClass = Defaults.Classes.Position.TopCenter,
        SnackbarVariant = Variant.Filled
    };
});

await builder.Build().RunAsync();