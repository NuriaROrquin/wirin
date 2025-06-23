using Wirin.Domain.Dtos.OCR;

namespace Wirin.Domain.Providers;

public interface IOcrProvider
{
    Task<OcrResultDto> ProcessWithEngineAsync(string engineName, FileStream file);

}
