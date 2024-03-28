
using Microsoft.AspNetCore.Mvc;

namespace test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class test2fwawfaController : ControllerBase
    {
        [HttpGet]
        [Route("test2awfawf")]
        public IActionResult Get()
        {
            return Ok("test2fwawfa response");
        }
    }
}