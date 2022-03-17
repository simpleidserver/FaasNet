import { createAction, props } from '@ngrx/store';
export const startGetAllVpn = createAction('[VPN] START_GET_ALL');
export const completeGetAllVpn = createAction('[VPN] COMPLETE_GET_ALL', props());
export const errorGetAllVpn = createAction('[VPN] ERROR_GET_ALL');
export const startAddVpn = createAction('[VPN] START_ADD_VPN', props());
export const completeAddVpn = createAction('[VPN] COMPLETE_ADD_VPN', props());
export const errorAddVpn = createAction('[VPN] ERROR_ADD_VPN');
export const deleteVpn = createAction('[VPN] START_DELETE_VPN', props());
export const completeDeleteVpn = createAction('[VPN] COMPLETE_DELETE_VPN', props());
export const errorDeleteVpn = createAction('[VPN] ERROR_DELETE_VPN');
//# sourceMappingURL=vpn.actions.js.map