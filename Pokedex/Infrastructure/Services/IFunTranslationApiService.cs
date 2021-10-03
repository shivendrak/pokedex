using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pokedex.Infrastructure.Services
{
    public interface IFunTranslationApiService
    {
        Task<string> Translate(string text, TranslationLanguage language);
    }
    
    public enum TranslationLanguage
    {
        Yoda, Shakespeare
    }
    
    public class TranslationRequest
    {
        public TranslationRequest(string text)
        {
            Text = text;
        }
        
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public class TranslationResponse
    {
        [JsonPropertyName("success")]
        public TranslationStat TranslationStat { get; set; }
        
        [JsonPropertyName("contents")]
        public TranslatedContent TranslatedContent { get; set; }
    }

    public class TranslationStat
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    public class TranslatedContent
    {
        [JsonPropertyName("translated")]
        public string Translated { get; set; }
        
        [JsonPropertyName("text")]
        public string OrignalText { get; set; }
        
        [JsonPropertyName("translation")]
        public string Type { get; set; }
    }
}