
namespace OpenAiToolsLibrary.Services;

public interface IEmbeddingService
{
    Task<float[]> GenerateEmbeddingAsync(IList<string> texts);
}