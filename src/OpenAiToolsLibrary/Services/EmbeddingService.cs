namespace OpenAiToolsLibrary.Services;

public class EmbeddingService(IOpenAiClientService openAiClient) : IEmbeddingService
{
    readonly OpenAIClient? _client = openAiClient.GetClient();

    public List<string> EmbeddingModels { get; private set; } = ["text-embedding-3-small", "text-embedding-3-large", "text-embedding-ada-002"];
    public string? SelectedEmbeddingModel { get; set; } = "text-embedding-ada-002";

    public async Task<float[]> GenerateEmbeddingAsync(IList<string> texts, string? embeddingModel = default)
    {
        if (_client == null) throw new NullReferenceException("Open AI client is null.");

        if (string.IsNullOrWhiteSpace(embeddingModel))
            embeddingModel = !string.IsNullOrWhiteSpace(SelectedEmbeddingModel) ? SelectedEmbeddingModel : EmbeddingModels.FirstOrDefault();

        var options = new EmbeddingsOptions(embeddingModel, texts);

        var response = await _client.GetEmbeddingsAsync(options);
        var embeddings = response.Value.Data
            .SelectMany(d => d.Embedding.ToArray())
            .ToArray();

        return embeddings;
    }
}