
using Microsoft.AspNetCore.Mvc;

namespace test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class testtestController : ControllerBase
    {
        [HttpGet]
        [Route("testtest")]
        public IActionResult Get()
        {
            return Ok("testtest response");
        }
    }
}