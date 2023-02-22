namespace ChatService.Storage.Entities;

public record ProfileEntity(string PartitionKey, string UserName, string FirstName, string LastName, string ProfilePictureId);
