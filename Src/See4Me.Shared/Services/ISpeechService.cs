using System.Threading.Tasks;

namespace See4Me.Services
{
    public interface ISpeechService
    {
        Task SpeechAsync(string text, bool queue = false, string languge = null);
    }
}