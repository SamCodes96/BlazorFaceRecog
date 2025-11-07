using System.Net.Mime;
using BlazorFaceRecog.Server;
using BlazorFaceRecog.Server.Configuration;
using BlazorFaceRecog.Server.Endpoints;
using BlazorFaceRecog.Server.Logic;
using BlazorFaceRecog.Server.Repositories;
using BlazorFaceRecog.Server.Services;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
    builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddResponseCompression(opts =>
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat([MediaTypeNames.Application.Octet]));

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

builder.Services.AddSignalR().AddHubOptions<FaceHub>(opt => opt.MaximumReceiveMessageSize = 1024 * 1024);
builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IFaceCache, FaceCache>();

builder.Services.AddTransient<IFaceService, FaceService>();
builder.Services.AddTransient<IFaceLogic, FaceLogic>();
builder.Services.AddTransient<IImageLogic, ImageLogic>();

builder.Services.AddFaceONNX();

builder.Services
    .AddOptions<AppSettings>()
    .Bind(builder.Configuration)
    .ValidateDataAnnotations();

var mongoSettings = builder.Configuration.GetSection("MongoDB");
if (mongoSettings.Exists())
   builder.Services.AddMongoDB(mongoSettings);
else
   builder.Services.AddSingleton<IFaceRepository, InMemoryFaceRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCompression();

app.UseHsts();
app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

app.MapFaceEndpoints();

app.MapRazorPages();
app.MapFallbackToFile("index.html");

await app.RunAsync();