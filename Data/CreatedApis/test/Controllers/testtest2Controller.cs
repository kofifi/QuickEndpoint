
using Microsoft.AspNetCore.Mvc;

namespace test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class testtest2Controller : ControllerBase
    {
        [HttpGet]
        [Route("testtest2")]
        public IActionResult Get()
        {
            return Ok("testtest2 response");
        }
    }
}