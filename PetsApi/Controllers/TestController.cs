using Microsoft.AspNetCore.Mvc;
using PetsApi.Data;

namespace PetsApi.Controllers
{
//#if DEBUG
    [ApiController]
    [Route("test")]
    public class TestController : Controller
    {
        private readonly AppDbContext _db;

        public TestController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("reset")]
        public async Task<IActionResult> Reset()
        {
            var pets = _db.Pets.Where(p => p.Name.StartsWith("Test"));

            _db.Pets.RemoveRange(pets);

            await _db.SaveChangesAsync();

            return Ok();

        }
    }
}
//#endif