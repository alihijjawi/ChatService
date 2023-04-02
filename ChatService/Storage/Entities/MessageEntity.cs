namespace ChatService.Storage.Entities;

public record MessageEntity(
    int id, 
    string ConversationId, 
    long UnixTime, 
    string SenderUsername,
    string Text);