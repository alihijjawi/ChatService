using ChatService.Settings;
using ChatService.Storage;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<CosmosSettings>(builder.Configuration.GetSection("Storage"));
builder.Services.Configure<StorageAccountSettings>(builder.Configuration.GetSection("Storage"));

builder.Services.AddSingleton<IProfileStore, CosmosProfileStore>();
builder.Services.AddSingleton(sp =>
{
    var cosmosOptions = sp.GetRequiredService<IOptions<CosmosSettings>>();
    return new CosmosClient(cosmosOptions.Value.ProfileDbString);
});

builder.Services.AddSingleton<IImageStore, ProfileImageStore>();
builder.Services.AddSingleton(sp =>
{
    var blobOptions = sp.GetRequiredService<IOptions<StorageAccountSettings>>();
    var storageAccount = CloudStorageAccount.Parse(blobOptions.Value.BlobStorageString);
    return storageAccount.CreateCloudBlobClient();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Chat Service API");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }