using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pokedex.Infrastructure;
using Pokedex.Infrastructure.Exceptions;
using Pokedex.Infrastructure.Services;

namespace Pokedex.Application.Pokemon
{
    public class PokemonDetailsHandler: IRequestHandler<PokemonDetailsRequest, PokemonDetailsResponse>
    {
        private readonly IPokeApiService _pokeApiService;
        private readonly IMapper _mapper;
        private readonly ILogger<PokemonDetailsHandler> _logger;
        private readonly IDistributedCache _cache;
        private readonly IFunTranslationApiService _translationApiService;

        public PokemonDetailsHandler(IPokeApiService pokeApiService, 
            IMapper mapper, ILogger<PokemonDetailsHandler> logger
            ,IDistributedCache cache, IFunTranslationApiService translationApiService)
        {
            _pokeApiService = pokeApiService;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
            _translationApiService = translationApiService;
        }
        public async Task<PokemonDetailsResponse> Handle(PokemonDetailsRequest request, CancellationToken cancellationToken)
        {
            var pokemonDetails = await GetPokemon(request.Name);
            if (!request.WithTranslation) return pokemonDetails;
            
            pokemonDetails.Description = await GetTranslation(pokemonDetails);
            return pokemonDetails;
        }
        
        private async Task<string> GetTranslation(PokemonDetailsResponse pokemon)
        {
            var cacheKey = string.Format(ApplicationConstants.TranslationCacheKey, pokemon.Name);
            var translation = await _cache.GetObjectAsync<string>(cacheKey);
            if (!string.IsNullOrEmpty(translation))
            {
                return translation;
            }
            var language = pokemon.IsLegendary || pokemon.Habitat == "cave"
                ? TranslationLanguage.Yoda
                : TranslationLanguage.Shakespeare;
            translation = await _translationApiService.Translate(pokemon.Description, language);
            await _cache.SetObjectAsync(cacheKey, translation);
            return translation;
        }
        
        private async Task<PokemonDetailsResponse> GetPokemon(string pokemonName)
        {
            _logger.LogInformation("Attempting to find pokemon in cache");
            var cachedPokemonDetails = await _cache.GetObjectAsync<PokemonDetailsResponse>(pokemonName);
            if (cachedPokemonDetails != null) return cachedPokemonDetails;
            
            _logger.LogInformation("Pokemon not found in cache");
            var pokemon = await _pokeApiService.GetPokemon(pokemonName);
            if (pokemon == null || string.IsNullOrEmpty(pokemon.Name)) 
                throw new DomainException($"Pokemon with name: {pokemonName} does not exist.");
            
            var pokemonDetails = _mapper.Map<PokemonDetailsResponse>(pokemon);
            await _cache.SetObjectAsync(pokemonName, pokemonDetails);
            return pokemonDetails;
        }

    }
}