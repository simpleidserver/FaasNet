using FaasNet.CRDT.Core.SerializedEntities;
using System.Linq;
using System.Text.Json;

namespace FaasNet.CRDT.Core.Entities
{
    public class CRDTEntitySerializer
    {
        public SerializedEntity Serialize(string id, CRDTEntity crdtEntity)
        {
            var type = crdtEntity.Name;
            var json = JsonSerializer.Serialize((object[])crdtEntity.ClockVector.ToArray(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return new SerializedEntity
            {
                Id = id,
                Type = type,
                Value = json
            };
        }
    }
}
