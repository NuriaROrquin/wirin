using Wirin.Domain.Dtos.OCR;

namespace Wirin.Infrastructure.Strategies.Interfaces;

public interface IOcrEngine
{
    string Name { get; }
    Task<OcrResultDto> ProcessPdfAsync(FileStream file);
}
