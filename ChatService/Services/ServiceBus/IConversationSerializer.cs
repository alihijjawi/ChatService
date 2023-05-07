using ChatService.Dtos;

namespace ChatService.Services.ServiceBus;

public interface IConversationSerializer
{
    string SerializeConversation(ConversationDto conversation);
    ConversationDto DeserializeConversation(string serialized);
}