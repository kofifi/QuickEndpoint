
using Microsoft.AspNetCore.Mvc;

namespace test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class test2tt2t2Controller : ControllerBase
    {
        [HttpGet]
        [Route("test22t2t2tt12t")]
        public IActionResult Get()
        {
            return Ok("test2tt2t2 response");
        }
    }
}