
namespace OpenAiToolsLibrary.Services;

public interface IChatGptService
{
    List<string> GptModels { get; }
    string? SelectedGptModel { get; set; }
    float Temperature { get; set; }
    int MaxTokens { get; set; }
    long Seed { get; set; }

    void AddRequestAssistantMessage(string? message);
    void AddRequestSystemMessage(string? message);
    void AddRequestUserMessage(string? message);
    string? ChatRequestMessagesToString(bool omitSystemMessages = false, bool omitUserMessages = false, bool omitAssistantMessages = false);
    void ClearChatRequestMessages();
    string? GetChatHistory();
    void RestoreChatRequestMessagesFromChatHistory(string? chatHistory);
    Task<string> SendRequestAsync(string? gptModel = null, float temperature = 0, int maxTokens = 0, long seed = 0);
}