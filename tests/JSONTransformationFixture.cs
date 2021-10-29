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
        public void TransformJSON(string input, string filter, string output)
        {
            var inputObj = JObject.Parse(input);
            var transformed = inputObj.Transform(filter);
            Assert.Equal(output, transformed.ToString());
        }

        [Theory]
        [InlineData(
            "{'message': 'firstname'}",
            "$.message",
            "lastname",
            "{\r\n  \"message\": \"lastname\"\r\n}")]
        [InlineData(
            "{'message': { 'content': 'firstname' }}",
            "$.message.content",
            "lastname",
            "{\r\n  \"message\": {\r\n    \"content\": \"lastname\"\r\n  }\r\n}")]
        [InlineData(
            "{'messages': ['firstInput']}",
            "$.messages",
            "secondInput",
            "{\r\n  \"messages\": [\r\n    \"firstInput\",\r\n    \"secondInput\"\r\n  ]\r\n}")]
        [InlineData(
            "{'messages': [ { 'content' : 'firstName'} ] }",
            "$.messages",
            "{ 'content': 'lastName' }",
            "{\r\n  \"messages\": [\r\n    {\r\n      \"content\": \"firstName\"\r\n    },\r\n    {\r\n      \"content\": \"lastName\"\r\n    }\r\n  ]\r\n}")]
        [InlineData(
            "{'messages': [ 'firstName' ] }",
            null,
            "{ 'messages' : [ 'lastName' ] }",
            "{\r\n  \"messages\": [\r\n    \"firstName\",\r\n    \"lastName\"\r\n  ]\r\n}")]
        public void MergeJSON(string input, string path, string value, string output)
        {
            var inputObj = JObject.Parse(input);
            var valueObj = GetJToken(value);
            inputObj.Merge(path, valueObj);
            Assert.Equal(output, inputObj.ToString());
        }

        private static JToken GetJToken(string str)
        {
            JToken result = str;
            try
            {
                result = JToken.Parse(str);
            }
            catch { }

            return result;
        }
    }
}
