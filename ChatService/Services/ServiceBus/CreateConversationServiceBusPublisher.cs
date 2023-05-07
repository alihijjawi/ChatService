using Azure.Messaging.ServiceBus;
using ChatService.Dtos;
using ChatService.Settings;
using Microsoft.Extensions.Options;

namespace ChatService.Services.ServiceBus;

public class CreateConversationServiceBusPublisher : ICreateConversationPublisher
{
    private readonly IConversationSerializer _conversationSerializer;
    private readonly ServiceBusSender _sender;

    public CreateConversationServiceBusPublisher(
        ServiceBusClient serviceBusClient,
        IConversationSerializer conversationSerializer,
        IOptions<ServiceBusSettings> options)
    {
        _conversationSerializer = conversationSerializer;
        _sender = serviceBusClient.CreateSender(options.Value.CreateConversationQueueName);
    }

    public Task Send(ConversationDto conversation)
    {
        var serialized = _conversationSerializer.SerializeConversation(conversation);
        return _sender.SendMessageAsync(new ServiceBusMessage(serialized));
    }
}