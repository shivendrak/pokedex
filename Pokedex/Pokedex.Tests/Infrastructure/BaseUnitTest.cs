using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Pokedex.Infrastructure.Services;

namespace Pokedex.Tests.Infrastructure
{
    public class BaseUnitTest
    {
        protected readonly Mock<IPokeApiService> _pokeApiService;
        protected readonly IMapper _mapper;
        
        private Dictionary<string, Pokemon> pokeMonData =
            new Dictionary<string, Pokemon>();
        
        protected Pokemon Pikachu = new Pokemon()
        {
            FlavorTextEntries = new List<FlavorTextEntry>()
            {
                new FlavorTextEntry() { Language = new Language() { Name = "en" }, Text = "This is sample description" }
            },
            Habitat = new Habitat() { Name = "jungle" }, Name = "pikachu", IsLegendary = false
        };
        
        protected BaseUnitTest()
        {
            pokeMonData["pikachu"] = Pikachu;

            _pokeApiService = new Mock<IPokeApiService>();
            _pokeApiService.Setup(_ => _.GetPokemon(It.IsAny<string>()))
                .Returns<string>((key)
                    => Task.FromResult<Pokemon>(pokeMonData.ContainsKey(key) ? pokeMonData[key] : null));

            var mapConfig = new MapperConfiguration(cfg => { cfg.AddProfile<PokemonMappingProfile>(); });
            _mapper = mapConfig.CreateMapper();
        }
    }
}