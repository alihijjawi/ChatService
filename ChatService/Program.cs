using Azure.Messaging.ServiceBus;
using ChatService.Services;
using ChatService.Services.ServiceBus;
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
builder.Services.Configure<ServiceBusSettings>(builder.Configuration.GetSection("ServiceBus"));

builder.Services.AddSingleton(sp =>
{
    var serviceBusOptions = sp.GetRequiredService<IOptions<ServiceBusSettings>>();
    return new ServiceBusClient(serviceBusOptions.Value.ConnectionString);
});

builder.Services.AddSingleton<IProfileStore, CosmosProfileStore>();
builder.Services.AddSingleton(sp =>
{
    var cosmosOptions = sp.GetRequiredService<IOptions<CosmosSettings>>();
    return new CosmosClient(cosmosOptions.Value.CosmosString);
});

builder.Services.AddSingleton<IImageStore, ProfileImageStore>();
builder.Services.AddSingleton(sp =>
{
    var blobOptions = sp.GetRequiredService<IOptions<StorageAccountSettings>>();
    var storageAccount = CloudStorageAccount.Parse(blobOptions.Value.BlobStorageString);
    return storageAccount.CreateCloudBlobClient();
});

builder.Services.AddSingleton<IConversationsStore, CosmosConversationsStore>();
builder.Services.AddSingleton(sp =>
{
    var cosmosOptions = sp.GetRequiredService<IOptions<CosmosSettings>>();
    return new CosmosClient(cosmosOptions.Value.CosmosString);
});

builder.Services.AddSingleton<IMessageStore, CosmosMessageStore>();
builder.Services.AddSingleton(sp =>
{
    var cosmosOptions = sp.GetRequiredService<IOptions<CosmosSettings>>();
    return new CosmosClient(cosmosOptions.Value.CosmosString);
});

builder.Services.AddSingleton<IChatManager, ChatManager>();
builder.Services.AddSingleton<IProfileService, ProfileService>();
builder.Services.AddSingleton<IImageService, ImageService>();
builder.Services.AddSingleton<IConversationsService, ConversationsService>();
builder.Services.AddSingleton<IMessageService, MessageService>();

builder.Services.AddSingleton<ICreateProfilePublisher, CreateProfileServiceBusPublisher>();
builder.Services.AddSingleton<IProfileSerializer, JsonProfileSerializer>();    
builder.Services.AddHostedService<CreateProfileHostedService>();

builder.Services.AddSingleton<ICreateConversationPublisher, CreateConversationServiceBusPublisher>();
builder.Services.AddSingleton<IConversationSerializer, JsonConversationSerializer>();    
builder.Services.AddHostedService<CreateConversationHostedService>();

builder.Services.AddApplicationInsightsTelemetry();

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