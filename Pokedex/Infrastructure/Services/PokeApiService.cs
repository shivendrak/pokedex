using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pokedex.Infrastructure.Exceptions;

namespace Pokedex.Infrastructure.Services
{
    public class PokeApiService : IPokeApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PokeApiService> _logger;

        public PokeApiService(IHttpClientFactory httpClientFactory, ILogger<PokeApiService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<Pokemon> GetPokemon(string name)
        {
            var client = _httpClientFactory.CreateClient(ApplicationConstants.PokeApiClientName);
            
            _logger.LogInformation("requesting pokemon inform from pokeapi");
            var response = await client.GetAsync(string.Format(ApplicationConstants.PokeApiSubUrl, name),
                new CancellationToken());
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Unable to fetch information for pokemon: {PokemonName}", name);
                _logger.LogDebug(response.ReasonPhrase);
                return response.StatusCode == HttpStatusCode.NotFound ? null 
                    : throw new Exception("something went wrong");
            }
            
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Pokemon>(jsonString);
        }
    }
}