using Brainbay.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brainbay.Domain.Services
{
    public interface ICharacterRepository
    {
        //web
        Task<IReadOnlyList<Character>> GetAllAsync();
        Task<Character> AddAsync(Character c);
        Task<IReadOnlyList<Character>> GetByOriginAsync(string planet);

        //console
        Task ClearAsync();
        Task SaveAsync(IEnumerable<Character> characters);
    }
}
