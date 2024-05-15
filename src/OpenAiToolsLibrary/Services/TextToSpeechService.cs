using Azure;
using System.Reflection;

namespace OpenAiToolsLibrary.Services;

public class TextToSpeechService(IOpenAiClientService openAiClient) : ITextToSpeechService
{
    readonly OpenAIClient? _client = openAiClient.GetClient();

    public List<SpeechVoice> Voices { get => GetVoices(); }
    public SpeechVoice SelectedVoice { get; set; }

    public async Task<MemoryStream> ConvertTextToAudio(string? text, SpeechVoice speechVoice = default)
    {
        if (_client == null) throw new NullReferenceException("Open AI client is null.");

        if (speechVoice == null)
        {
            if (SelectedVoice == null)
                SelectedVoice = Voices.FirstOrDefault();

            speechVoice = SelectedVoice;
        }
        

        SpeechGenerationOptions speechOptions = new()
        {
            Input = text,
            DeploymentName = "tts-1",
            Voice = speechVoice,
            ResponseFormat = SpeechGenerationResponseFormat.Aac,
            Speed = 1.0f,
        };

        Response<BinaryData> response = await _client.GenerateSpeechFromTextAsync(speechOptions);
        MemoryStream stream = new(response.Value.ToArray());

        return stream;
    }

    static List<SpeechVoice> GetVoices()
    {
        List<SpeechVoice>? speechVoices = typeof(SpeechVoice).GetProperties(BindingFlags.Public | BindingFlags.Static)
                                                            .Where(p => p.PropertyType == typeof(SpeechVoice))
                                                            .Select(p => p.GetValue(null))
                                                            .OfType<SpeechVoice>()
                                                            .ToList();

        if (speechVoices is null || speechVoices.Count == 0)
            throw new Exception("Could not get speech voices.");

        return speechVoices;
    }
}