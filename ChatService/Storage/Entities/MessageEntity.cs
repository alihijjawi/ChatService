namespace ChatService.Storage.Entities;

public record MessageEntity(
    string id, 
    string ConversationId, 
    long UnixTime, 
    string Sender,
    string Text);