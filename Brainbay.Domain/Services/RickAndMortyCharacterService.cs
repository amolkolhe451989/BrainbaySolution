using Brainbay.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Brainbay.Domain.Services
{
    public class RickAndMortyCharacterService : ICharacteropsService
    {
        private readonly HttpClient _client;

        public RickAndMortyCharacterService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<Character>> GetAliveCharactersAsync()
        {
            var aliveCharacters = new List<Character>();
            string? next = "character";

            do
            {
                var response = await _client.GetAsync(next);
                response.EnsureSuccessStatusCode();

                var payload = await response.Content.ReadFromJsonAsync<RmCharactersResponse>();
                if (payload is null) break;

                aliveCharacters.AddRange(payload.results
                    .Where(r => string.Equals(r.status, "Alive", StringComparison.OrdinalIgnoreCase))
                    .Select(r => new Character
                    {
                        ExternalId = r.id,
                        Name = r.name ?? "",
                        Status = r.status ?? "",
                        Species = r.species ?? "",
                        Gender = r.gender ?? "",
                        Origin = r.origin?.name ?? "",
                        Location = r.location?.name ?? "",
                        ImageUrl = r.image ?? ""
                    }));

                next = payload.info.next;
            } while (next != null);

            return aliveCharacters;
        }
    }
    public record RmCharactersResponse(RmInfo info, List<RmCharacter> results);
    public record RmInfo(string? next);
    public record RmCharacter(int id, string? name, string? status, string? species,
                              string? gender, RmNamed? origin, RmNamed? location, string? image);
    public record RmNamed(string? name);
}
