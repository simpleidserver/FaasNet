using FaasNet.Function.Attributes;

namespace FaasNet.Function.Transform
{
    public class Mapping
    {
        [Translation("fr", "Input")]
        [Translation("en", "Input")]
        public string Input { get; set; }
        [Translation("fr", "Output")]
        [Translation("en", "Output")]
        public string Output { get; set; }
    }
}
