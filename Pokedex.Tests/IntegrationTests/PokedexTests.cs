using FluentAssertions;
using Pokedex.Tests.Infrastructure;
using Xunit;

namespace Pokedex.Tests.IntegrationTests
{
    public class PokedexTests
    {
        private readonly PokedexClient _client = new();
        
        [Fact]
        public async void ShouldReturnValidPokemon()
        {
            var pokemonResponse = await _client.GetPokemon("pikachu", false);
            pokemonResponse.Should().NotBeNull();
            pokemonResponse.Name.Should().Be("pikachu");
        }
        
        [Fact]
        public async void ShouldReturnValidPokemonWithTranslation()
        {
            var pokemonResponse = await _client.GetPokemon("pikachu", true);
            pokemonResponse.Should().NotBeNull();
            pokemonResponse.Name.Should().Be("pikachu");
        }
    }
}