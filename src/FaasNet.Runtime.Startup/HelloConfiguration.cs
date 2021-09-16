using FaasNet.Runtime.Attributes;

namespace FaasNet.Runtime.Startup
{
    [FuncInfo("hello", "v1")]
    public class HelloConfiguration
    {
        [Translation("fr", "Est en majuscule")]
        [Translation("en", "Is bold")]
        public bool IsBold { get; set; }
    }
}