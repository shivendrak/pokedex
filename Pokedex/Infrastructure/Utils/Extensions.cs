using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Pokedex.Infrastructure
{
    public static class Extensions
    {
        public static async Task<T> GetObjectAsync<T>(this IDistributedCache cache, string key)
        {
            var obj = await cache.GetAsync(key);
            return obj == null ? default 
                : JsonSerializer.Deserialize<T>(new ReadOnlySpan<byte>(obj));
        }

        public static async Task SetObjectAsync<T>(this IDistributedCache cache, string key, T obj)
        {
            var objJson = JsonSerializer.SerializeToUtf8Bytes(obj);
            await cache.SetAsync(key, objJson);
        }
    }
}