using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Pokedex.Application.Pokemon;
using Pokedex.Infrastructure.Services;

namespace Pokedex.Tests.Infrastructure
{
    public class BaseUnitTest
    {
        protected Mock<IPokeApiService> PokeApiService;
        protected Mock<IFunTranslationApiService> TranslationService;
        protected Mock<IDistributedCache> Cache;
        protected IMapper _mapper;
        protected readonly Dictionary<string, Pokemon> PokeMonData = new();
        protected readonly Dictionary<string, byte[]> CacheData = new();
        protected readonly Dictionary<string, string> YodaDict = new();
        protected readonly Dictionary<string, string> ShakespeareDict = new();

        protected Pokemon Pikachu;
        protected Pokemon CavePokemon;
        protected Pokemon LegedaryPokemon;
        protected const string PlainDescription = "This is sample description";
        protected const string YodaDescription = "This is Yoda description";
        protected const string SakespeareDescription = "This is Sakespeare description";

        protected BaseUnitTest()
        {
            SetupSampleData();
            SetupCache();
            SetupPokeApi();
            SetupMapper();
            SetupTranslationApi();
        }

        private void SetupSampleData()
        {
            Pikachu = GetPokemon("pikachu");
            LegedaryPokemon = GetPokemon("uxie", legendary:true);
            CavePokemon = GetPokemon("zubat", "cave");
            
            PokeMonData["pikachu"] = Pikachu;
            PokeMonData["zubat"] = CavePokemon;
            PokeMonData["uxie"] = LegedaryPokemon;
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
        
        private void SetupTranslationApi()
        {
            ShakespeareDict.Add(PlainDescription, SakespeareDescription);
            YodaDict.Add(PlainDescription, YodaDescription);
            
            TranslationService = new Mock<IFunTranslationApiService>();
            TranslationService.Setup(_ => _.Translate(It.IsAny<string>(), It.IsAny<TranslationLanguage>()))
                .Returns<string, TranslationLanguage>((text, key) 
                    =>
                {
                    // To mock the translation failure.
                    if (text == "shouldfail") return Task.FromResult(text);
                    
                    // For rest of the cases.
                    return key switch
                    {
                        TranslationLanguage.Yoda when YodaDict.ContainsKey(text) 
                            => Task.FromResult(YodaDict[text]),
                        TranslationLanguage.Shakespeare when ShakespeareDict.ContainsKey(text) 
                            => Task.FromResult(ShakespeareDict[text]),
                        _ => Task.FromResult(string.Empty)
                    };
                });
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

        protected Pokemon GetPokemon(string name, string habitat = "jungle", string desc = PlainDescription, bool legendary = false)
        {
            return new Pokemon
            {
                FlavorTextEntries = new List<FlavorTextEntry>
                {
                    new() { Language = new Language { Name = "en" }, Text = desc }
                },
                Habitat = new Habitat { Name = habitat }, Name = name, IsLegendary = legendary
            };
        }
    }
}