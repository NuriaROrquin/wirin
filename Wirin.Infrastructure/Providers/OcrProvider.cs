using Wirin.Domain.Dtos.OCR;
using Wirin.Domain.Providers;
using Wirin.Infrastructure.Selectors.Interfaces;

namespace Wirin.Infrastructure.Providers;

public class OcrProvider : IOcrProvider
{
    private readonly IOcrEngineSelector _engineSelector;

    public OcrProvider(IOcrEngineSelector engineSelector)
    {
        _engineSelector = engineSelector;
    }

    public async Task<OcrResultDto> ProcessWithEngineAsync(string engineName, FileStream file)
    {
        var engine = _engineSelector.GetEngine(engineName);
        return await engine.ProcessPdfAsync(file);
    }
}