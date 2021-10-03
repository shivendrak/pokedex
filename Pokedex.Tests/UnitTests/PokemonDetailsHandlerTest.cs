using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Pokedex.Application.Pokemon;
using Pokedex.Infrastructure.Exceptions;
using Pokedex.Infrastructure.Services;
using Pokedex.Tests.Infrastructure;
using Xunit;

namespace Pokedex.Tests.UnitTests
{
    public class PokemonDetailsHandlerTest : BaseUnitTest
    {
        private readonly PokemonDetailsHandler _handler;
        private readonly Mock<ILogger<PokemonDetailsHandler>> _logger;

        public PokemonDetailsHandlerTest()
        {
            _logger = new Mock<ILogger<PokemonDetailsHandler>>();
            _handler = new PokemonDetailsHandler(PokeApiService.Object, _mapper, 
                _logger.Object, Cache.Object, TranslationService.Object);
        }

        [Fact]
        public async Task ShouldReturn_ValidPokemon_WhenInputNameIsCorrect()
        {
            var request = Given.PokemonRequest().WithName(Pikachu.Name);
            var pokemon = await _handler.Handle(request, CancellationToken.None);

            pokemon.Name.Should().Be(Pikachu.Name);
            pokemon.Description.Should().Be(Pikachu.FlavorTextEntries[0].Text);
            pokemon.Habitat.Should().Be(Pikachu.Habitat.Name);
            pokemon.IsLegendary.Should().Be(Pikachu.IsLegendary);

            // Should have been cached
            var isCached = CacheData.ContainsKey(Pikachu.Name);
            isCached.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldThrow_Exception_WhenInputNameIsNotValid()
        {
            var request = Given.PokemonRequest().WithName("invalid_pikachu");

            await FluentActions.Awaiting(async () => await _handler.Handle(request, CancellationToken.None))
                .Should().ThrowAsync<DomainException>();
        }
        
        [Fact]
        public async Task ShouldReturn_ValidPokemonFromCache_WhenSameInputIsReceived_SecondTime()
        {
            var request = Given.PokemonRequest().WithName(Pikachu.Name);
            await _handler.Handle(request, CancellationToken.None);
            
            // Should have been cached
            var isCached = CacheData.ContainsKey(Pikachu.Name);
            isCached.Should().BeTrue();

            var pokemon = await _handler.Handle(request, CancellationToken.None);
            PokeApiService.Verify(_=>_.GetPokemon(It.IsAny<string>()), Times.Once);
            pokemon.Should().NotBeNull();
        }
        
        [Fact]
        public async Task ShouldReturn_ValidPokeMon_WithTranslatedDescription()
        {
            var request = Given.PokemonRequest().WithName(Pikachu.Name).WithTranslation();
            var pokemon = await _handler.Handle(request, CancellationToken.None);

            pokemon.Name.Should().Be(Pikachu.Name);
            pokemon.Description.Should().Be(SakespeareDescription);
            pokemon.Habitat.Should().Be(Pikachu.Habitat.Name);
            pokemon.IsLegendary.Should().Be(Pikachu.IsLegendary);
        }
        
        [Fact]
        public async Task ShouldReturn_ValidTranslatedPokeMon_YodaLanguage()
        {
            // When Cave
            var request = Given.PokemonRequest().WithName(CavePokemon.Name).WithTranslation();
            var pokemon = await _handler.Handle(request, CancellationToken.None);

            pokemon.Name.Should().Be(CavePokemon.Name);
            pokemon.Description.Should().Be(YodaDescription);
            pokemon.Habitat.Should().Be(CavePokemon.Habitat.Name);
            pokemon.IsLegendary.Should().Be(CavePokemon.IsLegendary);
            
            // When Legendary
            request = Given.PokemonRequest().WithName(LegedaryPokemon.Name).WithTranslation();
            pokemon = await _handler.Handle(request, CancellationToken.None);

            pokemon.Name.Should().Be(LegedaryPokemon.Name);
            pokemon.Description.Should().Be(YodaDescription);
            pokemon.Habitat.Should().Be(LegedaryPokemon.Habitat.Name);
            pokemon.IsLegendary.Should().Be(LegedaryPokemon.IsLegendary);
        }
        
        [Fact]
        public async Task ShouldReturn_ValidPokeMon_WhenTranslationFails()
        {
            var pokemonDesc = "shouldfail";
            var pokemonName = "translationFailPokemon";
            var pokemon = GetPokemon(pokemonName, desc: pokemonDesc);
            PokeMonData.Add(pokemonName,pokemon);
            
            var request = Given.PokemonRequest().WithName(pokemonName).WithTranslation();
            var pokemonDetails = await _handler.Handle(request, CancellationToken.None);

            pokemonDetails.Should().NotBeNull();
            pokemonDetails.Description.Should().Be(pokemonDesc);
        }
        
        [Fact]
        public async Task ShouldReturn_ValidTranslatedPokeMon_FromCache()
        {
            var request = Given.PokemonRequest().WithName(Pikachu.Name).WithTranslation();
            await _handler.Handle(request, CancellationToken.None);
            
            var pokemon = await _handler.Handle(request, CancellationToken.None);
            PokeApiService.Verify(_=>_.GetPokemon(It.IsAny<string>()), Times.Once);
            pokemon.Should().NotBeNull();
        }
    }
}