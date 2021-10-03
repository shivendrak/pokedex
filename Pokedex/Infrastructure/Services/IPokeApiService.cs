using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pokedex.Infrastructure.Services
{
    public interface IPokeApiService
    {
        Task<Pokemon> GetPokemon(string name);
    }
    
    public class Pokemon
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("is_legendary")]
        public bool IsLegendary { get; set; }
        
        [JsonPropertyName("habitat")]
        public Habitat Habitat { get; set; }
        
        [JsonPropertyName("flavor_text_entries")]
        public IList<FlavorTextEntry> FlavorTextEntries { get; set; }
    }

    public class FlavorTextEntry
    {
        [JsonPropertyName("flavor_text")]
        public string Text { get; set; }
        
        [JsonPropertyName("language")]
        public Language Language { get; set; }
       
    }

    public class Language
    {
        [JsonPropertyName("name")]
        public string Name { get;set; }
        
        [JsonPropertyName("iso639")]
        public string ISO639 { get;set; }
    }

    public class Habitat
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}