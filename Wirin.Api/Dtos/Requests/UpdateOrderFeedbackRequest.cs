using System.ComponentModel.DataAnnotations;

namespace Wirin.Api.Dtos.Requests;

public class UpdateOrderFeedbackRequest
{
    [Required]
    public string StudentId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "El texto de feedback no puede estar vacío.")]
    public string FeedbackText { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Las estrellas deben estar entre 1 y 5.")]
    public int Stars { get; set; }

    [Required]
    public int OrderId { get; set; }
}