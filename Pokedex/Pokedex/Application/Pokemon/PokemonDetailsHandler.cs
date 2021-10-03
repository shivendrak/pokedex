using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Pokedex.Application.Pokemon
{
    public class PokemonDetailsHandler: IRequestHandler<PokemonDetailsRequest, PokemonDetailsResponse>
    {
        public Task<PokemonDetailsResponse> Handle(PokemonDetailsRequest request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}