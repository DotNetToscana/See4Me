using System.Threading.Tasks;

namespace See4Me.Services
{
    public interface ISpeechService
    {
        Task SpeechAsync(string text, string languge = null);
    }
}