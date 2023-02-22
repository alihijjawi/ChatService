using System.ComponentModel.DataAnnotations;

namespace ChatService.Dtos;

public record UploadImageRequest(
    [Required] IFormFile File);