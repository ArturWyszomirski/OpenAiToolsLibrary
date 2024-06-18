namespace OpenAiToolsLibrary.Services;

public class OpenAiClientService : IOpenAiClientService
{
    readonly OpenAIClient? _client;

    public OpenAiClientService(string apiKey)
    {
        _client = new(apiKey, new OpenAIClientOptions());

        if (_client == null) throw new NullReferenceException("Open AI client is null.");
    }

    public OpenAIClient? GetClient() => _client;
}