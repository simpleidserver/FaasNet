using System;

namespace FaasNet.EventMesh.Core.Vpn.Queries.Results
{
    public class VpnResult
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }

        public static VpnResult Build(Runtime.Models.Vpn vpn)
        {
            return new VpnResult
            {
                Name = vpn.Name,
                Description = vpn.Description,
                CreateDateTime = vpn.CreateDateTime,
                UpdateDateTime = vpn.UpdateDateTime
            };
        }
    }
}
