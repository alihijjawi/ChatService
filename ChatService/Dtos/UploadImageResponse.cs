using System.ComponentModel.DataAnnotations;

namespace ChatService.Dtos;

public record UploadImageResponse(
    [Required] string ImageId);