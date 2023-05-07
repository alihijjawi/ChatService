using ChatService.Dtos;
using Newtonsoft.Json;

namespace ChatService.Services.ServiceBus;

public class JsonConversationSerializer : IConversationSerializer
{
    public string SerializeConversation(ConversationDto conversation)
    {
        return JsonConvert.SerializeObject(conversation);
    }

    public ConversationDto DeserializeConversation(string serialized)
    {
        return JsonConvert.DeserializeObject<ConversationDto>(serialized);
    }
}