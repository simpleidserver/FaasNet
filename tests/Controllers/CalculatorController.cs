using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace FaasNet.Runtime.Tests.Controllers
{
    [Route("calculator")]
    public class CalculatorController : Controller
    {
        [HttpGet("{expr}", Name = "calculator")]
        public IActionResult Get(string expr)
        {
            double? firstOperand = null;
            double? secondOperand = null;
            string op = string.Empty;
            var regex = new Regex(@"[0-9\.]+|[^0-9\.]+");
            foreach(var match in regex.Matches(expr))
            {
                var value = match.ToString();
                if(double.TryParse(value, out double dv))
                {
                    if (firstOperand == null)
                    {
                        firstOperand = dv;
                    }
                    else
                    {
                        secondOperand = dv;
                    }
                }
                else
                {
                    op = value;
                }

                if (firstOperand != null && secondOperand != null)
                {
                    switch(op)
                    {
                        case "+":
                            firstOperand += secondOperand;
                            break;
                        case "-":
                            firstOperand -= secondOperand;
                            break;
                        case "x":
                            firstOperand *= secondOperand;
                            break;
                        case "/":
                            firstOperand /= secondOperand;
                            break;
                    }

                    secondOperand = null;
                }
            }

            return Ok(firstOperand.Value);
        }
    }
}
