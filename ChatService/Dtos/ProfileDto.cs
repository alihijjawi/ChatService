using System.ComponentModel.DataAnnotations;

namespace ChatService.Dtos;

public record ProfileDto(
    [Required] string UserName, 
    [Required] string FirstName, 
    [Required] string LastName,
    [Required] string ProfilePictureId);