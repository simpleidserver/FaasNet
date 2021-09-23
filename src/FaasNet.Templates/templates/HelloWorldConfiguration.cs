using FaasNet.Runtime.Attributes;

namespace FaasNet.Function
{
    [FuncInfo("HelloWorld", "v1")]
    public class HelloWorldConfiguration
    {
        [Translation("fr", "Titre")]
        [Translation("en", "Title")]
        public string FirstName { get; set; }
    }
}