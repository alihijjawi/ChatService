namespace ChatService.Storage.Entities;

public record ProfileEntity(string PartitionKey, string id, string FirstName, string LastName, string ProfilePictureId);
