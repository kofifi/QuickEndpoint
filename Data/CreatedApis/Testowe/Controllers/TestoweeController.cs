
using Microsoft.AspNetCore.Mvc;

namespace Testowe.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestoweeController : ControllerBase
    {
        [HttpGet]
        [Route("sample")]
        public IActionResult Get()
        {
            // Placeholder for the actual logic to be executed when the endpoint is called.
            return Ok("Testowee response");
        }
    }
}