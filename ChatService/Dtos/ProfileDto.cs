using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ChatService.Dtos;

public record ProfileDto(
    [Required] string UserName, 
    [Required] string FirstName, 
    [Required] string LastName,
    string? ProfilePictureId);