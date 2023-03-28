using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;

namespace ChatService.Dtos;

public record SendMessageResponse(
    [Required] UnixDateTime CreatedUnixTime);