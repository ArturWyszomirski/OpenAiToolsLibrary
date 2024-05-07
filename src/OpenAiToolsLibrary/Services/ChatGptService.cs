namespace OpenAiToolsLibrary.Services;

public class ChatGptService : IChatGptService
{
    readonly OpenAIClient _client;
    List<ChatRequestMessage>? _chatRequestMessages;

    public ChatGptService()
    {
        string apiKey = GetApiKey();
        _client = new(apiKey, new OpenAIClientOptions());
        _chatRequestMessages = [];
    }

    static string GetApiKey() => "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";

    public void ClearChatRequestMessages() => _chatRequestMessages?.Clear();
    public void AddRequestSystemMessage(string? message) => _chatRequestMessages?.Add(new ChatRequestSystemMessage(message));
    public void AddRequestUserMessage(string? message) => _chatRequestMessages?.Add(new ChatRequestUserMessage(message));
    public void AddRequestAssistantMessage(string? message) => _chatRequestMessages?.Add(new ChatRequestAssistantMessage(message));

    public async Task<string> SendRequestAsync(string gptModel = "gpt-3.5-turbo", float temperature = (float)0.1, int maxTokens = 500, long seed = 42)
    {
        ChatCompletionsOptions chatCompletionsOptions = new(gptModel, _chatRequestMessages)
        {
            Temperature = temperature,
            MaxTokens = maxTokens,
            Seed = seed
        };

        string assistantMessageContent = string.Empty;
        await foreach (StreamingChatCompletionsUpdate chatUpdate in _client.GetChatCompletionsStreaming(chatCompletionsOptions))
        {
            if (!string.IsNullOrEmpty(chatUpdate.ContentUpdate))
            {
                assistantMessageContent += $"{chatUpdate.ContentUpdate}";
            }
        }

        return assistantMessageContent;
    }

    public string? GetChatHistory()
    {
        //return JsonSerializer.Serialize(_chatRequestMessages);
        string json = JsonConvert.SerializeObject(_chatRequestMessages, Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects
        });

        return json;
    }

    public void RestoreChatRequestMessagesFromChatHistory(string? chatHistory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(chatHistory, nameof(chatHistory));

        //_chatRequestMessages = JsonSerializer.Deserialize<IList<ChatRequestMessage>>(chatHistory);
        _chatRequestMessages = JsonConvert.DeserializeObject<List<ChatRequestMessage>>(chatHistory, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects
        });
    }

    public string? ChatRequestMessagesToString(bool omitSystemMessages = false, bool omitUserMessages = false, bool omitAssistantMessages = false)
    {
        string chatRequestMessages = string.Empty;

        for (int messageNumber = 0; messageNumber < _chatRequestMessages?.Count; messageNumber++)
        {
            ChatRequestMessage? message = _chatRequestMessages[messageNumber];

            if (message is ChatRequestSystemMessage systemMessage && !omitSystemMessages)
                chatRequestMessages += $"{systemMessage.Role.ToString().ToUpperInvariant()}: {systemMessage.Content}";
            else if (message is ChatRequestUserMessage userMessage && !omitUserMessages)
                chatRequestMessages += $"{userMessage.Role.ToString().ToUpperInvariant()}: {userMessage.Content}";
            else if (message is ChatRequestAssistantMessage assistantMessage && !omitAssistantMessages)
                chatRequestMessages += $"{assistantMessage.Role.ToString().ToUpperInvariant()}: {assistantMessage.Content}";

            if (messageNumber != _chatRequestMessages?.Count - 1)
                chatRequestMessages += "\n\n";
        }

        return chatRequestMessages;
    }
}
