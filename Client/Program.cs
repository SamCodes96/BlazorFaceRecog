using BlazorFaceRecog.Client;
using BlazorFaceRecog.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("BlazorFaceRecog.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorFaceRecog.ServerAPI"));

builder.Services.AddScoped<FaceHubService>();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration = new()
    {
        PositionClass = Defaults.Classes.Position.TopCenter,
        SnackbarVariant = Variant.Filled
    };
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

await builder.Build().RunAsync();