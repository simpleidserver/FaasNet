using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FaasNet.StateMachine.Core.Tests.Controllers
{
    public class SendEmailPostParameter
    {
        [Required]
        public string Address { get; set; }
        [Required]
        public string Body { get; set; }
        public IEnumerable<string> Destinations { get; set; }
        public IEnumerable<Picture> Pictures { get; set; }
        public EmailInformationParameter Parameter { get; set; }
    }

    public class Picture
    {
        public string Url { get; set; }
    }

    public class EmailInformationParameter
    {
        [Required]
        public string Name { get; set; }
    }

    public class SendEmailGetParameter
    {
        [Required]
        [FromQuery(Name = "Address")]
        public string Address { get; set; }
        [Required]
        [FromQuery(Name = "Body")]
        public string Body { get; set; }
    }

    [Route("emails")]
    public class EmailsController : Controller
    {
        [HttpPost(Name = "sendEmailPost")]
        public IActionResult Send([FromBody] SendEmailPostParameter parameter)
        {
            return new OkObjectResult(new
            {
                SentDateTime = DateTime.UtcNow,
                Delivered = true
            });
        }

        [HttpGet(Name = "sendEmailGet")]
        public IActionResult SendGet([FromQuery] SendEmailGetParameter parameter)
        {
            return new OkObjectResult(new
            {
                SentDateTime = DateTime.UtcNow,
                Delivered = true
            });
        }
    }
}
