import { Action, createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/vpn.actions';
import { VpnResult } from '../models/vpn.model';

const selectedVpnName : string = "selectedVpn";
const selectedVpn = sessionStorage.getItem(selectedVpnName);

export interface VpnLstState {
  VpnLst: VpnResult[];
}

export interface VpnState {
  Vpn: VpnResult | null;
  selectedVpn: string | null;
}

export const initialVpnLstState: VpnLstState = {
  VpnLst: []
};

export const initialVpnState: VpnState = {
  Vpn: null,
  selectedVpn: selectedVpn
}

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

const vpnReducer = createReducer(
  initialVpnState,
  on(fromActions.completeGetVpn, (state, { content }) => {
    return {
      ...state,
      Vpn: { ...content }
    };
  }),
  on(fromActions.selectVpn, (state, { name }) => {
    sessionStorage.setItem(selectedVpnName, name);
    return {
      ...state,
      selectedVpn: name
    };
  }),
);

export function getVpnLstReducer(state: VpnLstState | undefined, action: Action) {
  return vpnLstReducer(state, action);
}

export function getVpnReducer(state: VpnState | undefined, action: Action) {
  return vpnReducer(state, action);
}
