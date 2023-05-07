using ChatService.Dtos;

namespace ChatService.Storage.Entities;

public record ConversationEntity(
    string id,
    ProfileDto Recipient,
    long LastModifiedUnixTime);