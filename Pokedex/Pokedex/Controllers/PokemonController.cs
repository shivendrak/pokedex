using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pokedex.Application.Pokemon;

namespace Pokedex.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PokemonController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PokemonController> _logger;

        public PokemonController(IMediator mediator, ILogger<PokemonController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        
        [HttpGet("{pokemonName}")]
        public async Task<PokemonDetailsResponse> Get(string pokemonName)
        {
            _logger.LogInformation("received request for pokemon: {PokemonName}", pokemonName );
            var request = new PokemonDetailsRequest(pokemonName);
            _logger.LogInformation("request forwarded to handler");
            return await _mediator.Send(request);
        }
    }
}