using System.Collections.Generic;
using System.Threading.Tasks;

namespace See4Me.Services.Translator
{
    public interface ITranslatorService
    {
        string ClientId { get; set; }

        string ClientSecret { get; set; }

        string Language { get; set; }

        Task<string> DetectLanguageAsync(string text);

        Task<IEnumerable<string>> GetLanguagesAsync();

        Task InitializeAsync();

        Task<string> TranslateAsync(string text, string to = null);
    }
}