using System.Threading.Tasks;

namespace See4Me.Services
{
    public interface ISpeechService
    {
        string Language { get; set; }

        Task SpeechAsync(string text, string languge = null);
    }
}