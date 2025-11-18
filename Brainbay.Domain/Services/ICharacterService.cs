using Brainbay.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brainbay.Domain.Services
{
    public interface ICharacteropsService
    {
        //console
        Task<List<Character>> GetAliveCharactersAsync();

    }
    public interface ICharacteropsService : ICharacteropsService
    {
        //web
        Task<(IReadOnlyList<Character> items, bool fromDatabase)> GetAllAsync();
        Task<Character> AddAsync(Character c);
        Task<IReadOnlyList<Character>> GetByOriginAsync(string planet);
    }
}