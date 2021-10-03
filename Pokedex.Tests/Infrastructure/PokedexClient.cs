using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Pokedex.Application.Pokemon;

namespace Pokedex.Tests.Infrastructure
{
    public class PokedexClient
    {
        private readonly HttpClient _httpClient;

        public PokedexClient()
        {
            // Arrange
            var testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _httpClient = testServer.CreateClient();
        }

        public async Task<PokemonDetailsResponse> GetPokemon(string name, bool withTranslation)
        {
            var url = withTranslation ? $"/pokemon/{name}" : $"pokemon/translated/{name}";
            var response = await _httpClient.GetAsync(url);
            var responseString =
                response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : string.Empty;
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return response.IsSuccessStatusCode 
                ? JsonSerializer.Deserialize<PokemonDetailsResponse>(responseString, options) : null;
        }
    }
}