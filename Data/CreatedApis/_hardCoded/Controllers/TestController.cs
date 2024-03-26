
using Microsoft.AspNetCore.Mvc;

namespace _hardCoded.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpDelete]
        [Route("sample")]
        public IActionResult Delete()
        {
            // Placeholder for the actual logic to be executed when the endpoint is called.
            return Ok("Test response");
        }
    }
}