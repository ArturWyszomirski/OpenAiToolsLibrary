
namespace OpenAiToolsLibrary.Services;

public interface IEmbeddingService
{
    Task<float[]> GenerateEmbeddingAsync(IList<string> texts, string? embeddingModel = default);

    public List<string> EmbeddingModels { get; }
    public string? SelectedEmbeddingModel { get; set; }
}