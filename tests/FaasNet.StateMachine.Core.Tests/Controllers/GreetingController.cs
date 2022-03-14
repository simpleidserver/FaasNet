using Microsoft.AspNetCore.Mvc;

namespace FaasNet.StateMachine.Core.Tests.Controllers
{
    [Route("greeting")]
    public class GreetingController : Controller
    {
        [HttpGet("{name}", Name = "greeting")]
        public IActionResult Get(string name)
        {
            return new OkObjectResult(new
            {
                result = $"Welcome to Serverless Workflow, {name}!"
            });
        }
    }
}
