using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;

namespace ChatService.Dtos;

public record StartConversationResponse(
    [Required] int ConversationId,
    [Required] UnixDateTime CreatedUnixTime);