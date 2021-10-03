using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Pokedex.Infrastructure.Exceptions;
using Pokedex.Infrastructure.Services;

namespace Pokedex.Application.Pokemon
{
    public class PokemonDetailsHandler: IRequestHandler<PokemonDetailsRequest, PokemonDetailsResponse>
    {
        private readonly IPokeApiService _pokeApiService;
        private readonly IMapper _mapper;
        private readonly ILogger<PokemonDetailsHandler> _logger;

        public PokemonDetailsHandler(IPokeApiService pokeApiService, IMapper mapper, ILogger<PokemonDetailsHandler> logger)
        {
            _pokeApiService = pokeApiService;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<PokemonDetailsResponse> Handle(PokemonDetailsRequest request, CancellationToken cancellationToken)
        {
            var pokemon = await _pokeApiService.GetPokemon(request.Name);
            if (pokemon == null || string.IsNullOrEmpty(pokemon.Name)) 
                throw new DomainException($"Pokemon with name: {request.Name} does not exist.");
            var pokemonDetails = _mapper.Map<PokemonDetailsResponse>(pokemon);
            return pokemonDetails;
        }

    }
}