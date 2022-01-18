using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace FaasNet.Runtime.Tests.Controllers
{
    public class SendEmailParameter
    {
        [Required]
        public string Address { get; set; }
        [Required]
        public string Body { get; set; }
        public EmailInformationParameter Parameter { get; set; }
    }

    public class EmailInformationParameter
    {
        [Required]
        public string Name { get; set; }
    }

    [Route("emails")]
    public class EmailsController : Controller
    {
        [HttpPost(Name = "sendEmail")]
        public IActionResult Send([FromBody] SendEmailParameter parameter)
        {
            return new OkObjectResult(new
            {
                SentDateTime = DateTime.UtcNow,
                Delivered = true
            });
        }
    }
}
