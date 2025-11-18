using Brainbay.Domain.Data;
using Brainbay.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brainbay.Domain.Services
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly AppDbContext _db;

        public CharacterRepository(AppDbContext db)
        {
            _db = db;
        }
        //console
        public async Task ClearAsync()
        {
            _db.Characters.RemoveRange(_db.Characters);
            await _db.SaveChangesAsync();
        }

        public async Task SaveAsync(IEnumerable<Character> characters)
        {
            await _db.Characters.AddRangeAsync(characters);
            await _db.SaveChangesAsync();
        }
        //web

        public async Task<IReadOnlyList<Character>> GetAllAsync()
        {
            return await _db.Characters
                            .AsNoTracking()
                            .OrderBy(c => c.Name)
                            .ToListAsync();
        }

        public async Task<Character> AddAsync(Character c)
        {
            await _db.Characters.AddAsync(c);
            await _db.SaveChangesAsync();
            return c;
        }

        public async Task<IReadOnlyList<Character>> GetByOriginAsync(string planet)
        {
            return await _db.Characters
                            .AsNoTracking()
                            .Where(c => c.Origin.ToLower() == planet.ToLower())
                            .OrderBy(c => c.Name)
                            .ToListAsync();
        }
    }
}
