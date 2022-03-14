using Microsoft.AspNetCore.Mvc;

namespace FaasNet.StateMachine.Core.Tests.Controllers
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
