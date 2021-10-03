using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Pokedex.Infrastructure.Services;

namespace Pokedex.Tests.Infrastructure
{
    public class BaseUnitTest
    {
        protected Mock<IPokeApiService> PokeApiService;
        protected Mock<IDistributedCache> Cache;
        protected IMapper _mapper;
        private readonly Dictionary<string, Pokemon> PokeMonData = new();
        protected readonly Dictionary<string, byte[]> CacheData = new();

        protected Pokemon Pikachu;
        
        protected BaseUnitTest()
        {
            SetupSampleData();
            SetupCache();
            SetupPokeApi();
            SetupMapper();
        }

        private void SetupSampleData()
        {
            Pikachu = new Pokemon
            {
                FlavorTextEntries = new List<FlavorTextEntry>
                {
                    new() { Language = new Language { Name = "en" }, Text = "This is sample description" }
                },
                Habitat = new Habitat { Name = "jungle" }, Name = "pikachu", IsLegendary = false
            };
            
            PokeMonData["pikachu"] = Pikachu;
        }

        private void SetupMapper()
        {
            var mapConfig = new MapperConfiguration(cfg => { cfg.AddProfile<PokemonMappingProfile>(); });
            _mapper = mapConfig.CreateMapper();
        }

        private void SetupPokeApi()
        {
            PokeApiService = new Mock<IPokeApiService>();
            PokeApiService.Setup(_ => _.GetPokemon(It.IsAny<string>()))
                .Returns<string>((key)
                    => Task.FromResult<Pokemon>(PokeMonData.ContainsKey(key) ? PokeMonData[key] : null));
        }

        private void SetupCache()
        {
            Cache = new Mock<IDistributedCache>();
            Cache.Setup(_ => _.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns<string, CancellationToken>((key, token)
                    => Task.FromResult<byte[]>(CacheData.ContainsKey(key) ? CacheData[key] : null));

            Cache.Setup(_ =>
                    _.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(),
                        It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>((key, value, arg, arg1) =>
                    CacheData.Add(key, value));
        }
    }
}