
using Microsoft.AspNetCore.Mvc;

namespace test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class test2test1Controller : ControllerBase
    {
        [HttpGet]
        [Route("test2test22")]
        public IActionResult Get()
        {
            return Ok("test2test1 response");
        }
    }
}