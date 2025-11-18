using Brainbay.Domain.Models;
using Brainbay.Domain.Services;
using Brainbay.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Brainbay.Web.Controllers
{
    [Route("characters")]
    public class CharactersController : Controller
    {
        private readonly CharacterService _service;
        public CharactersController(CharacterService service) => _service = service;

        // GET /characters
       [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var (items, fromDb) = await _service.GetAllAsync();
            var vm = items.Select(c => new CharacterViewModel
            {
                Id = c.Id,
                ExternalId = c.ExternalId,
                Name = c.Name,
                Status = c.Status,
                Species = c.Species,
                Gender = c.Gender,
                Origin = c.Origin,
                Location = c.Location,
                ImageUrl = c.ImageUrl
            });
            Response.Headers["from-database"] = fromDb ? "true" : "false";
            return View(vm);
        }

        // MVC view for create form
        [HttpGet("create")]
        public IActionResult Create() => View();

        [HttpPost("create")]
        public async Task<IActionResult> Create(CharacterViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var c = new Character
            {
                Name = vm.Name,
                Status = vm.Status,
                Species = vm.Species,
                Gender = vm.Gender,
                Origin = vm.Origin,
                Location = vm.Location,
                ImageUrl = vm.ImageUrl
            };

            await _service.AddAsync(c);
            return RedirectToAction(nameof(Index));
        }


        // BONUS: GET /characters/planet/{name}
        [HttpGet("planet/{name}")]
        public async Task<IActionResult> GetByPlanet([FromRoute] string name)
        {
            var items = await _service.GetByOriginAsync(name);
            Response.Headers["from-database"] = "true"; // direct DB query
            return Ok(items);
        }
    }
}