using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;

namespace ChatService.Dtos;

public record MessageDto(
    [Required] string Text,
    [Required] string SenderUsername, 
    [Required] long UnixTime);