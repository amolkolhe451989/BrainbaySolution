using Brainbay.Domain.Data;
using Brainbay.Domain.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Brainbay.Domain.Services
{
    //public class CharacterService
    //{
    //    private readonly AppDbContext _db;
    //    private readonly IMemoryCache _cache;
    //    private const string CacheKey = "characters_all";
    //    private static DateTimeOffset _lastAddUtc = DateTimeOffset.MinValue;

    //    public CharacterService(AppDbContext db, IMemoryCache cache)
    //    {
    //        _db = db;
    //        _cache = cache;
    //    }

    //    public async Task<(IReadOnlyList<Character> items, bool fromDatabase)> GetAllAsync()
    //    {
    //        // Try to get cached list
    //        var cacheExists = _cache.TryGetValue(CacheKey, out List<Character>? cached);

    //        // Try to get cached timestamp
    //        var cachedStamp = _cache.Get<DateTimeOffset?>($"{CacheKey}_stamp");
    //        var cacheAgeOk = cachedStamp.HasValue &&
    //                         (DateTimeOffset.UtcNow - cachedStamp.Value) <= TimeSpan.FromMinutes(5);

    //        // If cache exists, is fresh (<=5m), and no new adds since stamp, return cache
    //        if (cacheExists && cached is not null && cacheAgeOk && cachedStamp >= _lastAddUtc)
    //        {
    //            return (cached, false); // from cache
    //        }

    //        // Otherwise, query DB
    //        var data = await _db.Characters
    //                            .AsNoTracking()
    //                            .OrderBy(c => c.Name)
    //                            .ToListAsync();

    //        // Update cache with 5-minute policy
    //        _cache.Set(CacheKey, data, new MemoryCacheEntryOptions
    //        {
    //            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    //        });
    //        _cache.Set($"{CacheKey}_stamp", DateTimeOffset.UtcNow, new MemoryCacheEntryOptions
    //        {
    //            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    //        });

    //        return (data, true); // from DB
    //    }


    //    public async Task<Character> AddAsync(Character c)
    //    {
    //        await _db.Characters.AddAsync(c);
    //        await _db.SaveChangesAsync();
    //        _lastAddUtc = DateTimeOffset.UtcNow; // mark addition to bypass cache
    //        _cache.Remove(CacheKey);
    //        _cache.Remove($"{CacheKey}_stamp");
    //        return c;
    //    }

    //    public async Task<IReadOnlyList<Character>> GetByOriginAsync(string planet)
    //    {
    //        return await _db.Characters.AsNoTracking()
    //            .Where(c => c.Origin.ToLower() == planet.ToLower())
    //            .OrderBy(c => c.Name)
    //            .ToListAsync();
    //    }
    //}
    public class CharacterService : ICharacteropsService
    {
        private readonly ICharacterRepository _repo;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "characters_all";
        private static DateTimeOffset _lastAddUtc = DateTimeOffset.MinValue;

        public CharacterService(ICharacterRepository repo, IMemoryCache cache)
        {
            _repo = repo;
            _cache = cache;
        }

        public async Task<(IReadOnlyList<Character> items, bool fromDatabase)> GetAllAsync()
        {
            var cacheExists = _cache.TryGetValue(CacheKey, out List<Character>? cached);
            var cachedStamp = _cache.Get<DateTimeOffset?>($"{CacheKey}_stamp");
            var cacheAgeOk = cachedStamp.HasValue &&
                             (DateTimeOffset.UtcNow - cachedStamp.Value) <= TimeSpan.FromMinutes(5);

            if (cacheExists && cached is not null && cacheAgeOk && cachedStamp >= _lastAddUtc)
            {
                return (cached, false); // from cache
            }

            var data = await _repo.GetAllAsync();

            _cache.Set(CacheKey, data, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
            _cache.Set($"{CacheKey}_stamp", DateTimeOffset.UtcNow, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

            return (data, true); // from DB
        }

        public async Task<Character> AddAsync(Character c)
        {
            var added = await _repo.AddAsync(c);
            _lastAddUtc = DateTimeOffset.UtcNow;
            _cache.Remove(CacheKey);
            _cache.Remove($"{CacheKey}_stamp");
            return added;
        }

        public async Task<IReadOnlyList<Character>> GetByOriginAsync(string planet)
        {
            return await _repo.GetByOriginAsync(planet);
        }
    }

}
