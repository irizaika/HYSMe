using Microsoft.AspNetCore.Mvc;
using PetsApi.Data;

namespace PetsApi.Controllers
{
    [ApiController]
    [Route("test")]
    public class TestController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public TestController(
            AppDbContext db,
            IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [HttpPost("reset")]
        public async Task<IActionResult> Reset()
        {

            if (!_env.IsEnvironment("Testing") && !_env.IsEnvironment("Development"))
            {
                return NotFound();
            }
            var pets = _db.Pets.Where(p => p.Name.StartsWith("Test"));

            _db.Pets.RemoveRange(pets);

            await _db.SaveChangesAsync();

            return Ok();

        }
    }
}