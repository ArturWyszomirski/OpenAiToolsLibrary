namespace OpenAiToolsLibrary.Services;

public class OpenAiClientService : IOpenAiClientService
{
    OpenAIClient? _client;

    public OpenAIClient GetClient()
    {
        if (_client == null)
        {
            string apiKey = GetApiKey();
            _client = new(apiKey, new OpenAIClientOptions());
        }

        if (_client == null) throw new NullReferenceException("Open AI client is null.");

        return _client;
    }

    static string GetApiKey() => "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";
}