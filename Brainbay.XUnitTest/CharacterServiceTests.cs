using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Brainbay.Domain.Data;
using Brainbay.Domain.Models;
using Brainbay.Domain.Services;
namespace Brainbay.XUnitTest
{
    public class CharacterServiceTests
    {
        private readonly Mock<ICharacterRepository> _repoMock;
        private readonly IMemoryCache _cache;
        private readonly CharacterService _service;

        public CharacterServiceTests()
        {
            _repoMock = new Mock<ICharacterRepository>();
            _cache = new MemoryCache(new MemoryCacheOptions());
            _service = new CharacterService(_repoMock.Object, _cache);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsFromDatabase_WhenCacheIsEmpty()
        {
            var characters = new List<Character> { new Character { Id = 1, Name = "Rick" } };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(characters);

            var (items, fromDb) = await _service.GetAllAsync();

            Assert.True(fromDb);
            Assert.Single(items);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsFromCache_WhenCacheIsValid()
        {
            var characters = new List<Character> { new Character { Id = 1, Name = "Morty" } };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(characters);

            await _service.GetAllAsync();

            var (items, fromDb) = await _service.GetAllAsync();

            Assert.False(fromDb);
            Assert.Single(items);
        }

        [Fact]
        public async Task AddAsync_InvalidatesCache()
        {
            var character = new Character { Id = 2, Name = "Summer" };
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Character>())).ReturnsAsync(character);

            _cache.Set("characters_all", new List<Character> { new Character { Id = 1, Name = "Beth" } });
            _cache.Set("characters_all_stamp", DateTimeOffset.UtcNow);

            var added = await _service.AddAsync(character);

            Assert.Equal("Summer", added.Name);
            Assert.False(_cache.TryGetValue("characters_all", out _)); // cache should be cleared
        }
    }
}