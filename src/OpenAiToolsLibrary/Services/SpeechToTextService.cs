using Azure;

namespace OpenAiToolsLibrary.Services;

public class SpeechToTextService(IOpenAiClientService openAiClient) : ISpeechToTextService
{
    readonly OpenAIClient? _client = openAiClient.GetClient();

    public async Task<string?> SendTranscribeRequestAsync(Stream? audioStream)
    {
        if (_client == null) throw new NullReferenceException("Open AI client is null.");

        if (audioStream == null)
            return null;

        var transcriptionOptions = new AudioTranscriptionOptions()
        {
            DeploymentName = "whisper-1",
            AudioData = BinaryData.FromStream(audioStream),
            ResponseFormat = AudioTranscriptionFormat.Verbose,
        };

        Response<AudioTranscription>? transcriptionResponse = await _client.GetAudioTranscriptionAsync(transcriptionOptions);
        AudioTranscription? transcription = transcriptionResponse?.Value;

        return transcription?.Text;
    }
}