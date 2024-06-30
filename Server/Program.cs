using BlazorFaceRecog.Server.Hubs;
using BlazorFaceRecog.Server.Logic;
using BlazorFaceRecog.Server.Repositories;
using BlazorFaceRecog.Server.Services;
using FaceONNX;
using Microsoft.AspNetCore.ResponseCompression;
using MongoDB.Driver;
using ONNXOptions = Microsoft.ML.OnnxRuntime.SessionOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddResponseCompression(opts => opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream"]));

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<CacheService>();

builder.Services.AddSingleton<FaceService>();
builder.Services.AddSingleton<FaceLogic>();
builder.Services.AddSingleton<FaceRepository>();

builder.Services.AddSingleton(builder.Configuration.GetValue<bool>("UseGPU")
    ? ONNXOptions.MakeSessionOptionWithCudaProvider()
    : new ONNXOptions());

builder.Services.AddSingleton<IFace68LandmarksExtractor, Face68LandmarksExtractor>();
builder.Services.AddSingleton<IFaceDetector, FaceDetector>();
builder.Services.AddSingleton<IFaceClassifier, FaceEmbedder>();

var mongoClient = new MongoClient(builder.Configuration.GetConnectionString("MongoDB"));
builder.Services.AddSingleton(mongoClient.GetDatabase(builder.Configuration.GetSection("MongoDb")["DatabaseName"]));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCompression();

app.UseHsts();
app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

app.MapHub<FaceHub>($"/{nameof(FaceHub)}");
app.MapControllers();
app.MapRazorPages();
app.MapFallbackToFile("index.html");

app.Run();