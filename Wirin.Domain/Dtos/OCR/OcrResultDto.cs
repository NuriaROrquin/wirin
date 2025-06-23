namespace Wirin.Domain.Dtos.OCR;

public class OcrResultDto
{
    public string Status { get; set; } = "éxito";
    public string Message { get; set; } = "PDF procesado correctamente con Tesseract OCR.";
    public OcrMetadataDto Metadata { get; set; } = new OcrMetadataDto();
    public List<OcrPageResultDto> Pages { get; set; } = new List<OcrPageResultDto>();
    public string FullText { get; set; } = string.Empty;
}
