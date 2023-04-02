using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;

namespace ChatService.Dtos;

public record MessagesList(
    [Required] MessageDto[] Messages,
    [Required] string NextUrl);