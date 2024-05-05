namespace OpenAiToolsLibrary;

public class ChatGptService : IChatGptService
{
    readonly OpenAIClient _client;
    readonly IList<ChatRequestMessage> _chatRequestMessages;

    public ChatGptService()
    {
        string apiKey = GetApiKey();
        _client = new(apiKey, new OpenAIClientOptions());
        _chatRequestMessages = [];
    }

    static string GetApiKey() => "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";

    public void ClearChatRequestMessages() => _chatRequestMessages.Clear();
    public void AddRequestSystemMessage(string message) => _chatRequestMessages.Add(new ChatRequestSystemMessage(message));
    public void AddRequestUserMessage(string message) => _chatRequestMessages.Add(new ChatRequestUserMessage(message));
    public void AddRequestAssistantMessage(string message) => _chatRequestMessages.Add(new ChatRequestAssistantMessage(message));

    public async Task<string> SendRequestAsync(string gptModel = "gpt-3.5-turbo", float temperature = (float)0.1, int maxTokens = 500, long seed = 42)
    {
        ChatCompletionsOptions chatCompletionsOptions = new(gptModel, _chatRequestMessages)
        {
            Temperature = temperature,
            MaxTokens = maxTokens,
            Seed = seed
        };

        string assistantMessageContent = String.Empty;
        await foreach (StreamingChatCompletionsUpdate chatUpdate in _client.GetChatCompletionsStreaming(chatCompletionsOptions))
        {
            if (!string.IsNullOrEmpty(chatUpdate.ContentUpdate))
            {
                assistantMessageContent += $"{chatUpdate.ContentUpdate}";
            }
        }

        return assistantMessageContent;
    }
}
