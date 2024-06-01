namespace OpenAiToolsLibrary.Services;

public class EmbeddingService(IOpenAiClientService openAiClient) : IEmbeddingService
{
    readonly OpenAIClient? _client = openAiClient.GetClient();

    public async Task<float[]> GenerateEmbeddingAsync(IList<string> texts)
    {
        if (_client == null) throw new NullReferenceException("Open AI client is null.");

        var options = new EmbeddingsOptions("text-embedding-3-small", texts);//"text-embedding-ada-002"

        var response = await _client.GetEmbeddingsAsync(options);
        var embeddings = response.Value.Data
            .SelectMany(d => d.Embedding.ToArray())
            .ToArray();

        return embeddings;
    }
}