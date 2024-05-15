
namespace OpenAiToolsLibrary.Services;

public interface ITextToSpeechService
{
    List<SpeechVoice> Voices { get; }
    SpeechVoice SelectedVoice { get; set; }

    Task<MemoryStream> ConvertTextToAudio(string? text, SpeechVoice speechVoice = default);
}