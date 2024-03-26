
using Microsoft.AspNetCore.Mvc;

namespace Testowe.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestttController : ControllerBase
    {
        [HttpDelete]
        [Route("sample")]
        public IActionResult Delete()
        {
            // Placeholder for the actual logic to be executed when the endpoint is called.
            return Ok("Testtt response");
        }
    }
}