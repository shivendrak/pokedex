using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Pokedex.Application.Pokemon;
using Pokedex.Infrastructure.Exceptions;
using Pokedex.Tests.Infrastructure;
using Xunit;

namespace Pokedex.Tests.UnitTests
{
    public class PokemonDetailsHandlerTest : BaseUnitTest
    {
        private PokemonDetailsHandler _handler;
        private Mock<ILogger<PokemonDetailsHandler>> _logger;

        public PokemonDetailsHandlerTest()
        {
            _logger = new Mock<ILogger<PokemonDetailsHandler>>();
            _handler = new PokemonDetailsHandler( _pokeApiService.Object,_mapper, _logger.Object);
            
        }

        [Fact]
        public async Task ShouldReturnData()
        {
            var request = Given.PokemonRequest().WithName(Pikachu.Name);
            var pokemon = await _handler.Handle(request, CancellationToken.None);
            
            pokemon.Name.Should().Be(Pikachu.Name);
            pokemon.Description.Should().Be(Pikachu.FlavorTextEntries[0].Text);
            pokemon.Habitat.Should().Be(Pikachu.Habitat.Name);
            pokemon.IsLegendary.Should().Be(Pikachu.IsLegendary);
        }
        
        [Fact]
        public async Task ShouldThrowExceptionWhenInvalidPokemon()
        {
            var request = Given.PokemonRequest().WithName("invalid_pikachu");
            
            await FluentActions.Awaiting(async () => await _handler.Handle(request, CancellationToken.None))
                .Should().ThrowAsync<DomainException>();
        }
    }
}