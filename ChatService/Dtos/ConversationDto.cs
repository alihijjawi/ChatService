using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;

namespace ChatService.Dtos;

public record ConversationDto(
    [Required] string Id,
    [Required] UnixDateTime LastModifiedUnixTime,
    [Required] ProfileDto Recipient);