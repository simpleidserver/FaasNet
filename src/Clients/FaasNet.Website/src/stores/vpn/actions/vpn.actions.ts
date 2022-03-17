import { createAction, props } from '@ngrx/store';
import { VpnResult } from '../models/vpn.model';

export const startGetAllVpn = createAction('[VPN] START_GET_ALL');
export const completeGetAllVpn = createAction('[VPN] COMPLETE_GET_ALL', props<{ content : VpnResult[] }>());
export const errorGetAllVpn = createAction('[VPN] ERROR_GET_ALL');
export const startAddVpn = createAction('[VPN] START_ADD_VPN', props<{ name: string, description: string }>());
export const completeAddVpn = createAction('[VPN] COMPLETE_ADD_VPN', props<{ name: string, description: string}>());
export const errorAddVpn = createAction('[VPN] ERROR_ADD_VPN');
export const deleteVpn = createAction('[VPN] START_DELETE_VPN', props<{ name: string }>());
export const completeDeleteVpn = createAction('[VPN] COMPLETE_DELETE_VPN', props<{ name: string }>());
export const errorDeleteVpn = createAction('[VPN] ERROR_DELETE_VPN');
