using Wirin.Infrastructure.Selectors.Interfaces;
using Wirin.Infrastructure.Strategies.Interfaces;
using Wirin.Infrastructure.Strategies.Local;


namespace Wirin.Infrastructure.Selectors;

public class OcrEngineSelector : IOcrEngineSelector
{
   
    private readonly LocalOcrEngine _localEngine;

    public OcrEngineSelector(LocalOcrEngine localEngine)
    {
        
        _localEngine = localEngine;
    }

    public IOcrEngine GetEngine(string engineName)
    {
        return 
                engineName.Equals("Local", StringComparison.OrdinalIgnoreCase) 
                ? _localEngine 
                : throw new InvalidOperationException($"OCR engine '{engineName}' no encontrado.");
    }
}
