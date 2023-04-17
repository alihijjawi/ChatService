using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;

namespace ChatService.Dtos;

public record StartConversationResponse(
    [Required] string Id,
    [Required] long CreatedUnixTime);