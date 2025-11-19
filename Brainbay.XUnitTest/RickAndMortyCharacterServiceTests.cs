using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using Brainbay.Domain.Services;
namespace Brainbay.XUnitTest
{
    public class RickAndMortyCharacterServiceTests
    {
        private class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly HttpResponseMessage _response;

            public FakeHttpMessageHandler(HttpResponseMessage response)
            {
                _response = response;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(_response);
            }
        }

        [Fact]
        public async Task GetAliveCharactersAsync_ReturnsOnlyAliveCharacters()
        {
            // Arrange: fake API payload with Alive + Dead characters
            var payload = new RmCharactersResponse(
                new RmInfo(null), // no next page
                new List<RmCharacter>
                {
                new RmCharacter(1, "Rick", "Alive", "Human", "Male", new RmNamed("Earth"), new RmNamed("Earth"), "rick.png"),
                new RmCharacter(2, "Morty", "Dead", "Human", "Male", new RmNamed("Earth"), new RmNamed("Earth"), "morty.png")
                }
            );

            var json = JsonSerializer.Serialize(payload);
            var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };

            var handler = new FakeHttpMessageHandler(fakeResponse);
            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://rickandmortyapi.com/api/")
            };

            var service = new RickAndMortyCharacterService(client);

            // Act
            var aliveCharacters = await service.GetAliveCharactersAsync();

            // Assert
            Assert.Single(aliveCharacters); // only Rick should be returned
            Assert.Equal("Rick", aliveCharacters[0].Name);
            Assert.Equal("Alive", aliveCharacters[0].Status);
        }
    }
}
