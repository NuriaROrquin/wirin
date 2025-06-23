namespace Wirin.Domain.Dtos.OCR;

public class OcrImageDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int PageNumber { get; set; }
    public int OrderInPage { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public double Confidence { get; set; }
    public string Base64Data { get; set; } = string.Empty;
    public string Format { get; set; } = "png";
    public string Description { get; set; } = string.Empty; // Descripción de la imagen que puede ser editada en el frontend
}