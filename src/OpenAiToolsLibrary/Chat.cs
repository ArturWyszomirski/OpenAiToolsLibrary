namespace OpenAiToolsLibrary;

public class Chat(string apiKey) : IChat
{
    readonly OpenAIClient _client = new(apiKey, new OpenAIClientOptions());

    public async Task<string> SendRequestAsync(
        IList<ChatRequestMessage>? chatRequestMessages, string gptModel = "gpt-3.5-turbo", float temperature = (float)0.1, int maxTokens = 500, long seed = 42)
    {
        ChatCompletionsOptions chatCompletionsOptions = new(gptModel, chatRequestMessages)
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
