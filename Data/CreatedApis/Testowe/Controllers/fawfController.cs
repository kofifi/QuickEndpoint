
using Microsoft.AspNetCore.Mvc;

namespace Testowe.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class fawfController : ControllerBase
    {
        [HttpPut]
        [Route("fawfwa")]
        public IActionResult Put()
        {
            return Ok("fawf response");
        }
    }
}