
using Microsoft.AspNetCore.Mvc;

namespace test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class testfwafwaController : ControllerBase
    {
        [HttpGet]
        [Route("testswafaw")]
        public IActionResult Get()
        {
            return Ok("testfwafwa response");
        }
    }
}