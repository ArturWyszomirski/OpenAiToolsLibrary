namespace OpenAiToolsLibrary.Services;

public class ChatGptService : IChatGptService
{
    readonly OpenAIClient? _client;
    readonly List<ChatRequestMessage> _chatRequestMessages;
    readonly JsonSerializerOptions _serializerOptions;

    public ChatGptService(IOpenAiClientService openAiClient)
    {
        _client = openAiClient.GetClient();
        _chatRequestMessages = [];
        _serializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
        };

        GptModels = ["gpt-3.5-turbo", "gpt-4", "gpt-4-turbo-preview"];
        SelectedGptModel = GptModels.FirstOrDefault();
    }

    public List<string> GptModels { get; private set; }
    public string? SelectedGptModel { get; set; }
    public float Temperature { get; set; }
    public int MaxTokens { get; set; }
    public long Seed { get; set; }

    public void ClearChatRequestMessages() => _chatRequestMessages.Clear();
    public void AddRequestSystemMessage(string? message) => _chatRequestMessages.Add(new ChatRequestSystemMessage(message));
    public void AddRequestUserMessage(string? message) => _chatRequestMessages.Add(new ChatRequestUserMessage(message));
    public void AddRequestAssistantMessage(string? message) => _chatRequestMessages.Add(new ChatRequestAssistantMessage(message));

    public async Task<string> SendRequestAsync(string? gptModel = default, float temperature = default, int maxTokens = default, long seed = default)
    {
        if (_client == null) throw new NullReferenceException("Open AI client is null.");

        if (gptModel == default)
            gptModel = !string.IsNullOrEmpty(SelectedGptModel) ? SelectedGptModel : GptModels.FirstOrDefault();

        ChatCompletionsOptions chatCompletionsOptions = new(gptModel, _chatRequestMessages);

        if (temperature != default)
            chatCompletionsOptions.Temperature = temperature;
        else if (Temperature != default)
            chatCompletionsOptions.Temperature = Temperature;

        if (maxTokens != default)
            chatCompletionsOptions.MaxTokens = maxTokens;
        else if (MaxTokens != default)
            chatCompletionsOptions.MaxTokens = MaxTokens;

        if (seed != default)
            chatCompletionsOptions.Seed = seed;
        else if (Seed != default)
            chatCompletionsOptions.Seed = Seed;

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
        /* issue with deserialization: User and Assistant Message have protected set on Content and it is not populated in deserialization
        //string json = JsonConvert.SerializeObject(_chatRequestMessages, Formatting.Indented, new JsonSerializerSettings
        //{
        //    TypeNameHandling = TypeNameHandling.Objects
        //});
        */

        List<Message> chatHistory = [];

        for (int messageNumber = 0; messageNumber < _chatRequestMessages?.Count; messageNumber++)
        {
            ChatRequestMessage? message = _chatRequestMessages[messageNumber];

            if (message is ChatRequestSystemMessage systemMessage)
                chatHistory.Add(new($"{nameof(ChatRequestSystemMessage)}", systemMessage.Content));
            else if (message is ChatRequestUserMessage userMessage)
                chatHistory.Add(new($"{nameof(ChatRequestUserMessage)}", userMessage.Content));
            else if (message is ChatRequestAssistantMessage assistantMessage)
                chatHistory.Add(new($"{nameof(ChatRequestAssistantMessage)}", assistantMessage.Content));
        }

        string json = JsonSerializer.Serialize(chatHistory, _serializerOptions);

        return json;
    }

    public void RestoreChatRequestMessagesFromChatHistory(string? chatHistory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(chatHistory, nameof(chatHistory));

        /* issue with deserialization: User and Assistant Message have protected set on Content and it is not populated in deserialization
        //_chatRequestMessages = JsonConvert.DeserializeObject<List<ChatRequestMessage>>(chatHistory, new JsonSerializerSettings
        //{
        //    TypeNameHandling = TypeNameHandling.Objects
        //});
        */

        _chatRequestMessages.Clear();

        List<Message>? chatHistoryList = JsonSerializer.Deserialize<List<Message>>(chatHistory);

        if (chatHistoryList != null)
        {
            foreach (var message in chatHistoryList)
            {
                if (message.Type == $"{nameof(ChatRequestSystemMessage)}")
                    _chatRequestMessages.Add(new ChatRequestSystemMessage(message.Content));
                else if (message.Type == $"{nameof(ChatRequestUserMessage)}")
                    _chatRequestMessages.Add(new ChatRequestUserMessage(message.Content));
                else if (message.Type == $"{nameof(ChatRequestAssistantMessage)}")
                    _chatRequestMessages.Add(new ChatRequestAssistantMessage(message.Content));
            }
        }

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

    struct Message()
    {
        public Message(string type, string content) : this()
        {
            Type = type;
            Content = content;
        }

        public string? Type { get; set; }
        public string? Content { get; set; }
    }
}