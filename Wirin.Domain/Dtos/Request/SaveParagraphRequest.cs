namespace Wirin.Infrastructure.Dtos.Requests;

public class SaveParagraphRequest
{
    public int orderId { get; set; } 
    public string paragraphText { get; set; }
    public int pageNumber { get; set; }
    public bool? hasError { get; set; }
    public string? errorMessage { get; set; }
    public double? confidence { get; set; }

}
