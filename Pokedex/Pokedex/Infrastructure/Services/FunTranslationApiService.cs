using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pokedex.Infrastructure.Services
{
    public class FunTranslationApiService : IFunTranslationApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public FunTranslationApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<string> Translate(string text, TranslationLanguage language)
        {
            var client = _httpClientFactory.CreateClient(ApplicationConstants.TranslationApiClientName);
            var endpoint = language == TranslationLanguage.Shakespeare ? "shakespeare" : "yoda";

            var translationReq = new TranslationRequest(text);
            var strTranslationReq = JsonSerializer.Serialize(translationReq);
            var content = new StringContent(strTranslationReq, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endpoint, content);
            if (!response.IsSuccessStatusCode) return text;
            var jsonString = await response.Content.ReadAsStringAsync();
            var translation = JsonSerializer.Deserialize<TranslationResponse>(jsonString);
            return string.IsNullOrEmpty(translation.TranslatedContent.Translated) 
                ? translation.TranslatedContent.Translated 
                : text;
        }
    }
}