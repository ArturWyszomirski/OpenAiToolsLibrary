
namespace OpenAiToolsLibrary;

public interface IChat
{
    Task<string> SendRequestAsync(IList<ChatRequestMessage>? chatRequestMessages, string gptModel = "gpt-3.5-turbo", float temperature = 0.1F, int maxTokens = 500, long seed = 42);
}