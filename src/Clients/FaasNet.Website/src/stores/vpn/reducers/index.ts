import { Action, createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/vpn.actions';
import { VpnResult } from '../models/vpn.model';

export interface VpnLstState {
  VpnLst: VpnResult[];
}

export const initialVpnLstState: VpnLstState = {
  VpnLst: []
};

const vpnLstReducer = createReducer(
  initialVpnLstState,
  on(fromActions.completeGetAllVpn, (state, { content }) => {
    return {
      ...state,
      VpnLst: [...content]
    };
  }),
  on(fromActions.completeAddVpn, (state, { name, description }) => {
    const result = new VpnResult();
    result.name = name;
    result.description = description;
    result.createDateTime = new Date();
    result.updateDateTime = new Date();
    const vpnLst = JSON.parse(JSON.stringify(state.VpnLst)) as VpnResult[];
    vpnLst.push(result);
    return {
      ...state,
      VpnLst: [...vpnLst]
    };
  }),
  on(fromActions.completeDeleteVpn, (state, { name }) => {
    let vpnLst = JSON.parse(JSON.stringify(state.VpnLst)) as VpnResult[];
    vpnLst = vpnLst.filter(v => v.name !== name);
    return {
      ...state,
      VpnLst: [...vpnLst]
    };
  })
);

export function getVpnLstReducer(state: VpnLstState | undefined, action: Action) {
  return vpnLstReducer(state, action);
}
