using FaasNet.StateMachine.Runtime.Serializer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FaasNet.StateMachine.SqlServer.Startup.Infrastructure
{
    public class YamlInputFormatter : TextInputFormatter
    {
        public YamlInputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/yaml"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            var request = context.HttpContext.Request;
            using (var streamReader = context.ReaderFactory(request.Body, encoding))
            {
                var type = context.ModelType;
                try
                {
                    var content = await streamReader.ReadToEndAsync();
                    var deserializer = new DeserializerBuilder()
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .WithTypeConverter(new YamlWorkflowDefinitionStateConverter())
                        .WithTypeConverter(new YamlEnumConverter())
                        .WithTypeConverter(new YamlJObjectConverter())
                        .Build();
                    var model = deserializer.Deserialize(content, type);
                    return await InputFormatterResult.SuccessAsync(model);
                }
                catch
                {
                    return await InputFormatterResult.FailureAsync();
                }
            }
        }
    }
}
