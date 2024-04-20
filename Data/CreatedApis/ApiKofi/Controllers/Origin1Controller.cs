
    using Microsoft.AspNetCore.Mvc;

    namespace ApiKofi.Controllers
    {
        [ApiController]
        [Route("[controller]")]
        public class Origin1Controller : ControllerBase
        {
            [HttpGet("Endpoint1/Path1")]
            public IActionResult GetPath1()
            {
                // Logic for Path1
                return Ok("Response from Path1");
            }
        
            [HttpPost("Endpoint2/Path2")]
            public IActionResult GetPath2()
            {
                // Logic for Path2
                return Ok("Response from Path2");
            }
}
}