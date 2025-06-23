namespace Wirin.Domain.Dtos.OCR;

public class OcrStatisticsDto
{
    public int TotalCharacters { get; set; }
    public int TotalWords { get; set; }
    public double AverageCharactersPerPage { get; set; }
    public double AverageWordsPerPage { get; set; }
    public double AverageConfidence { get; set; }
}
