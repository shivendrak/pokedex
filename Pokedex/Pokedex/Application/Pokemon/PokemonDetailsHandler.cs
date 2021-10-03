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

        public PokemonDetailsHandler(IPokeApiService pokeApiService, 
            IMapper mapper, ILogger<PokemonDetailsHandler> logger
            , IDistributedCache cache)
        {
            _pokeApiService = pokeApiService;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }
        public async Task<PokemonDetailsResponse> Handle(PokemonDetailsRequest request, CancellationToken cancellationToken)
        {
            var cachedPokemonDetails = await _cache.GetObjectAsync<PokemonDetailsResponse>(request.Name);
            if (cachedPokemonDetails != null) return cachedPokemonDetails;
            
            var pokemon = await _pokeApiService.GetPokemon(request.Name);
            if (pokemon == null || string.IsNullOrEmpty(pokemon.Name)) 
                throw new DomainException($"Pokemon with name: {request.Name} does not exist.");
            
            var pokemonDetails = _mapper.Map<PokemonDetailsResponse>(pokemon);
            await _cache.SetObjectAsync(request.Name, pokemon);
            return pokemonDetails;
        }

    }
}