using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.Vpns
{
    [FeatureState]
    public class VpnsState
    {
        public VpnsState()
        {

        }

        public GenericSearchQueryResult<VpnQueryResult> Vpns { get; set; } = new GenericSearchQueryResult<VpnQueryResult>();
        public bool IsLoading { get; set; }

        public VpnsState(bool isLoading, GenericSearchQueryResult<VpnQueryResult> vpns)
        {
            IsLoading = isLoading;
            Vpns = vpns;
        }
    }
}
