namespace OpenAiToolsLibrary.Services;

public interface ISpeechToTextService
{
    Task<string?> SendTranscribeRequestAsync(Stream? audioStream);
}
