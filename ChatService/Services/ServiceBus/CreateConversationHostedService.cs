using Azure.Messaging.ServiceBus;
using ChatService.Settings;
using Microsoft.Extensions.Options;

namespace ChatService.Services.ServiceBus;

public class CreateConversationHostedService : IHostedService
{
    private readonly IConversationsService _conversationService;
    private readonly IConversationSerializer _conversationSerializer;
    private readonly ServiceBusProcessor _processor;

    public CreateConversationHostedService(
        ServiceBusClient serviceBusClient, 
        IConversationsService conversationService,
        IConversationSerializer conversationSerializer,
        IOptions<ServiceBusSettings> options)
    {
        _conversationService = conversationService;
        _conversationSerializer = conversationSerializer;
        _processor = serviceBusClient.CreateProcessor(options.Value.CreateConversationQueueName);
        
        // add handler to process messages
        _processor.ProcessMessageAsync += MessageHandler;

        // add handler to process any errors
        _processor.ProcessErrorAsync += ErrorHandler;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return _processor.StartProcessingAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _processor.StopProcessingAsync(cancellationToken);
    }
    
    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        string data = args.Message.Body.ToString();
        Console.WriteLine($"Received: {data}");

        var conversation = _conversationSerializer.DeserializeConversation(data);
        await _conversationService.CreateConversation(conversation.Id, conversation.Recipient, conversation.LastModifiedUnixTime);

        await args.CompleteMessageAsync(args.Message);
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
}