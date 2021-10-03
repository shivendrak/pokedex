using Pokedex.Application.Pokemon;

namespace Pokedex.Tests.Infrastructure
{
    public static class Given
    {
        public static PokemonDetailsRequest PokemonRequest()
        {
            var request = new PokemonDetailsRequest(string.Empty);
            return request;
        }
        
        public static PokemonDetailsRequest WithName(this PokemonDetailsRequest request
            , string name) 
        {
            request.Name = name;
            return request;
        }
    }
}