using FaasNet.CRDT.Core.SerializedEntities;
using System.Text.Json;

namespace FaasNet.CRDT.Core.Entities
{
    public class CRDTEntitySerializer
    {
        public SerializedEntity Serialize(string id, CRDTEntity crdtEntity)
        {
            var type = crdtEntity.Name;
            var json = JsonSerializer.Serialize(crdtEntity.ClockVector, new JsonSerializerOptions
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
