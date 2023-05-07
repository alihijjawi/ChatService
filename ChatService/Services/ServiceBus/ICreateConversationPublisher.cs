using ChatService.Dtos;

namespace ChatService.Services.ServiceBus;

public interface ICreateConversationPublisher
{
    Task Send(ConversationDto conversation);
}
