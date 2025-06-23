using Wirin.Infrastructure.Strategies.Interfaces;

namespace Wirin.Infrastructure.Selectors.Interfaces;

public interface IOcrEngineSelector
{
    IOcrEngine GetEngine(string engineName);
}
