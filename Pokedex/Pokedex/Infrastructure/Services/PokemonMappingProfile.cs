using System.Linq;
using AutoMapper;
using Pokedex.Application.Pokemon;

namespace Pokedex.Infrastructure.Services
{
    public class PokemonMappingProfile : Profile
    {
        public PokemonMappingProfile()
        {
            CreateMap<Pokemon, PokemonDetailsResponse>()
                .ForMember(pService => pService.Habitat,
                    pRes =>
                        pRes.MapFrom(o => o.Habitat
                            .Name))
                .ForMember(pService => pService.Description,
                    pRes =>
                        pRes.MapFrom(o => 
                            o.FlavorTextEntries
                                .FirstOrDefault(fte => fte.Language.Name == "en" 
                                                       && !string.IsNullOrEmpty(fte.Text)).Text));
        }
    }
}