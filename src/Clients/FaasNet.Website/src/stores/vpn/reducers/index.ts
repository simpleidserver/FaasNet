import { Action, createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/vpn.actions';
import { ClientResult } from '../models/client.model';
import { VpnResult } from '../models/vpn.model';

export interface VpnLstState {
  VpnLst: VpnResult[];
}

export interface ClientLstState {
  ClientLst: ClientResult[];
}

export interface ClientState {
  Client: ClientResult | null;
}

export interface VpnState {
  Vpn: VpnResult | null;
}

export const initialVpnLstState: VpnLstState = {
  VpnLst: []
};

export const initialClientLstState: ClientLstState = {
  ClientLst : []
};

export const initialClientState: ClientState = {
  Client : null
};

export const initialVpnState: VpnState = {
  Vpn: null
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
  })
);

const clientLstReducer = createReducer(
  initialClientLstState,
  on(fromActions.completeGetAllClients, (state, { content }) => {
    return {
      ...state,
      ClientLst: [...content]
    };
  }),
  on(fromActions.completeAddClient, (state, { name, clientId, purposes }) => {
    const clientLst = JSON.parse(JSON.stringify(state.ClientLst)) as ClientResult[];
    var record = new ClientResult();
    record.clientId = clientId;
    record.purposes = purposes;
    record.createDateTime = new Date();
    clientLst.push(record);
    return {
      ...state,
      ClientLst: [...clientLst]
    };
  }),
  on(fromActions.completeDeleteClient, (state, { name, clientId }) => {
    let clientLst = JSON.parse(JSON.stringify(state.ClientLst)) as ClientResult[];
    clientLst = clientLst.filter(c => c.clientId !== clientId);
    return {
      ...state,
      ClientLst: [...clientLst]
    };
  })
);

export function getVpnLstReducer(state: VpnLstState | undefined, action: Action) {
  return vpnLstReducer(state, action);
}

export function getVpnReducer(state: VpnState | undefined, action: Action) {
  return vpnReducer(state, action);
}

export function getClientLstReducer(state: ClientLstState | undefined, action: Action) {
  return clientLstReducer(state, action);
}
