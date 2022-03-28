import { Action, createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/clients.actions';
import { ClientResult } from '../models/client.model';

export interface ClientLstState {
  ClientLst: ClientResult[];
}

export interface ClientState {
  Client: ClientResult | null;
}

export const initialClientLstState: ClientLstState = {
  ClientLst : []
};

export const initialClientState: ClientState = {
  Client : null
};

const clientLstReducer = createReducer(
  initialClientLstState,
  on(fromActions.completeGetAllClients, (state, { content }) => {
    return {
      ...state,
      ClientLst: [...content]
    };
  }),
  on(fromActions.completeAddClient, (state, { id, vpn, clientId, purposes }) => {
    const clientLst = JSON.parse(JSON.stringify(state.ClientLst)) as ClientResult[];
    var record = new ClientResult();
    record.id = id;
    record.vpn = vpn;
    record.clientId = clientId;
    record.purposes = purposes;
    record.createDateTime = new Date();
    clientLst.push(record);
    return {
      ...state,
      ClientLst: [...clientLst]
    };
  }),
  on(fromActions.completeDeleteClient, (state, { id }) => {
    let clientLst = JSON.parse(JSON.stringify(state.ClientLst)) as ClientResult[];
    clientLst = clientLst.filter(c => c.id !== id);
    return {
      ...state,
      ClientLst: [...clientLst]
    };
  })
);

const clientReducer = createReducer(
  initialClientState,
  on(fromActions.completeGetClient, (state, { content }) => {
    return {
      ...state,
      Client: { ...content }
    };
  })
);

export function getClientLstReducer(state: ClientLstState | undefined, action: Action) {
  return clientLstReducer(state, action);
}

export function getClientReducer(state: ClientState | undefined, action: Action) {
  return clientReducer(state, action);
}
