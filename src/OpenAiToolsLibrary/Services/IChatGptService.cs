namespace OpenAiToolsLibrary.Services;

public interface IChatGptService
{
    void ClearChatRequestMessages();
    void AddRequestAssistantMessage(string? message);
    void AddRequestSystemMessage(string? message);
    void AddRequestUserMessage(string? message);
    Task<string> SendRequestAsync(string gptModel = "gpt-3.5-turbo", float temperature = 0.1F, int maxTokens = 500, long seed = 42);
    string? GetChatHistory();
    void RestoreChatRequestMessagesFromChatHistory(string? chatHistory);
    string? ChatRequestMessagesToString(bool omitSystemMessages = false, bool omitUserMessages = false, bool omitAssistantMessages = false);
}