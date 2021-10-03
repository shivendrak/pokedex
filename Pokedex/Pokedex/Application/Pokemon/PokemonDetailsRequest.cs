using MediatR;

namespace Pokedex.Application.Pokemon
{
    public class PokemonDetailsRequest : IRequest<PokemonDetailsResponse>
    {
        public PokemonDetailsRequest(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
        public bool WithTranslation { get; set; }
    }
    
    public class PokemonDetailsResponse
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Habitat { get; set; }
        public bool IsLegendary { get; set; }
    }
}