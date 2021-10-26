using FaasNet.Runtime.Extensions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace FaasNet.Runtime.Tests
{
    public class JSONTransformationFixture
    {

        [Theory]
        [InlineData(
            "{ 'fruits': ['apple', 'orange', 'pear'], 'vegetables': [{'veggieName': 'potatato', 'veggieLike': true}] }", 
            "{'vegetables': '$.vegetables' }", 
            "{\r\n  \"vegetables\": [\r\n    {\r\n      \"veggieName\": \"potatato\",\r\n      \"veggieLike\": true\r\n    }\r\n  ]\r\n}")]
        [InlineData(
            "{ 'fruits': ['apple', 'orange', 'pear'], 'vegetables': [{'veggieName': 'potatato', 'veggieLike': true}, {'veggieName': 'broccoli', 'veggieLike': false}] }",
            "{'vegetables': '$.vegetables[?(@.veggieLike == true)]' }",
            "{\r\n  \"vegetables\": {\r\n    \"veggieName\": \"potatato\",\r\n    \"veggieLike\": true\r\n  }\r\n}")]
        [InlineData(
            "{ 'firstname': 'SimpleIdServer' }",
            "{'result': 'Hello $.firstname' }",
            "{\r\n  \"result\": \"Hello SimpleIdServer\"\r\n}")]
        public void ExtractJSON(string input, string filter, string output)
        {
            var inputObj = JObject.Parse(input);
            var transformed = inputObj.Transform(filter);
            Assert.Equal(output, transformed);
        }
    }
}
