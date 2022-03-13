using FaasNet.Function.Attributes;

namespace Function
{
    [FuncInfo("HelloWorld", "v1")]
    public class HelloWorldConfiguration
    {
        [Translation("fr", "Titre")]
        [Translation("en", "Title")]
        public string FirstName { get; set; }
    }
}