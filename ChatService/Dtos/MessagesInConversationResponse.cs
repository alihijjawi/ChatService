using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;

namespace ChatService.Dtos;

public record MessagesInConversationResponse(
    [Required] MessageDto[] Messages,
    [Required] string NextUri);