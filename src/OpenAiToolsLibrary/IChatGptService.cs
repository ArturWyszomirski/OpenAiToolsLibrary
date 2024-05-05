
namespace OpenAiToolsLibrary;

public interface IChatGptService
{
    void AddRequestAssistantMessage(string message);
    void AddRequestSystemMessage(string message);
    void AddRequestUserMessage(string message);
    void ClearChatRequestMessages();
    Task<string> SendRequestAsync(string gptModel = "gpt-3.5-turbo", float temperature = 0.1F, int maxTokens = 500, long seed = 42);
}