using BlazorFaceRecog.Server.Configuration;
using BlazorFaceRecog.Server.Hubs;
using BlazorFaceRecog.Server.Logic;
using BlazorFaceRecog.Server.Repositories;
using BlazorFaceRecog.Server.Services;
using FaceONNX;
using Microsoft.AspNetCore.ResponseCompression;
using MongoDB.Bson.Serialization.Conventions;
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

builder.Services.AddSingleton(builder.Configuration.GetValue<bool>("UseGPU")
    ? ONNXOptions.MakeSessionOptionWithCudaProvider()
    : new ONNXOptions());

builder.Services.AddSingleton<IFace68LandmarksExtractor, Face68LandmarksExtractor>();
builder.Services.AddSingleton<IFaceDetector, FaceDetector>();
builder.Services.AddSingleton<IFaceClassifier, FaceEmbedder>();

var mongoSettings = builder.Configuration.GetSection("MongoDB").Get<MongoDBSettings>();
if (mongoSettings is not null)
{
    builder.Services.AddSingleton(mongoSettings);
    builder.Services.AddSingleton<IFaceRepository, MongoFaceRepository>();

    var mongoClient = new MongoClient(mongoSettings.ConnectionString);
    var mongoDatabase = mongoClient.GetDatabase(mongoSettings.DatabaseName);
    builder.Services.AddSingleton(mongoDatabase);

    // This stops MongoDB throwing an error because there isn't an _id field on the models
    ConventionRegistry.Register("IgnoreExtraElements", new ConventionPack { new IgnoreExtraElementsConvention(true) }, _ => true);
}
else
{
    builder.Services.AddSingleton<IFaceRepository, InMemoryFaceRepository>();
}

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