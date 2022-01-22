using FaasNet.Runtime.AsyncAPI.v2.Converters;
using Newtonsoft.Json;

namespace FaasNet.Runtime.AsyncAPI.v2.Models
{
    [JsonConverter(typeof(MessageConverter))]
    public interface IMessage
    {
    }
}
