using Microsoft.AspNetCore.Mvc;

namespace FaasNet.Runtime.Tests.Controllers
{
    [Route("credit")]
    public class CreditController : Controller
    {
        [HttpGet(Name = "creditCheck")]
        public IActionResult Check()
        {
            return Ok();
        }
    }
}
