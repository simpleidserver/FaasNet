import { Action, createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/vpn.actions';
import { AppDomainResult } from '../models/appdomain.model';
import { ClientResult } from '../models/client.model';
import { MessageDefinitionResult } from '../models/messagedefinition.model';
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

export interface ApplicationDomainLstState {
  ApplicationDomainLst: AppDomainResult[];
}

export interface ApplicationDomainState {
  ApplicationDomain: AppDomainResult | null;
}

export interface MessageDefinitionLstState {
  MessageDefinitionLst: MessageDefinitionResult[];
}

export interface MessageDefinitionState {
  MessageDefinition: MessageDefinitionResult | null;
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

export const initialMessageDefinitionLstState: MessageDefinitionLstState = {
  MessageDefinitionLst: []
};

export const initialMessageDefinitionState: MessageDefinitionState = {
  MessageDefinition : null
};

export const initialApplicationDomainLstState: ApplicationDomainLstState = {
  ApplicationDomainLst: []
};

export const initialApplicationDomainState: ApplicationDomainState = {
  ApplicationDomain: null
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

const clientReducer = createReducer(
  initialClientState,
  on(fromActions.completeGetClient, (state, { content }) => {
    return {
      ...state,
      Client: { ...content }
    };
  })
);

const applicationDomainLstReducer = createReducer(
  initialApplicationDomainLstState,
  on(fromActions.completeGetAppDomains, (state, { content }) => {
    return {
      ...state,
      ApplicationDomainLst: [...content]
    };
  }),
  on(fromActions.completeAddAppDomain, (state, { id, name, description, rootTopic }) => {
    const appDomainLst = JSON.parse(JSON.stringify(state.ApplicationDomainLst)) as AppDomainResult[];
    var record = new AppDomainResult();
    record.id = id;
    record.name = name;
    record.description = description;
    record.rootTopic = rootTopic;
    record.createDateTime = new Date();
    record.updateDateTime = new Date();
    appDomainLst.push(record);
    return {
      ...state,
      ApplicationDomainLst: [...appDomainLst]
    };
  }),
  on(fromActions.completeDeleteAppDomain, (state, { name, appDomainId }) => {
    let appDomainLst = JSON.parse(JSON.stringify(state.ApplicationDomainLst)) as AppDomainResult[];
    appDomainLst = appDomainLst.filter(c => c.id !== appDomainId);
    return {
      ...state,
      ApplicationDomainLst: [...appDomainLst]
    };
  })
);

const applicationDomainReducer = createReducer(
  initialApplicationDomainState,
  on(fromActions.completeGetAppDomain, (state, { content }) => {
    return {
      ...state,
      ApplicationDomain: { ...content }
    };
  })
);

const messageDefLstReducer = createReducer(
  initialMessageDefinitionLstState,
  on(fromActions.completeGetLatestMessages, (state, { content }) => {
    return {
      ...state,
      MessageDefinitionLst: [...content]
    };
  }),
  on(fromActions.completeAddMessageDefinition, (state, { vpn, messageDefId, applicationDomainId, name, description, jsonSchema }) => {
    const messageDefLst = JSON.parse(JSON.stringify(state.MessageDefinitionLst)) as MessageDefinitionResult[];
    var record = new MessageDefinitionResult();
    record.id = messageDefId;
    record.name = name;
    record.description = description;
    record.version = 0;
    record.createDateTime = new Date();
    record.updateDateTime = new Date();
    messageDefLst.push(record);
    return {
      ...state,
      MessageDefinitionLst: [...messageDefLst]
    };
  }),
  on(fromActions.completeUpdateMessageDefinition, (state, { vpn, applicationDomainId, messageDefId, description, jsonSchema }) => {
    let messageDefLst = JSON.parse(JSON.stringify(state.MessageDefinitionLst)) as MessageDefinitionResult[];
    const messageDef = messageDefLst.filter(m => m.id === messageDefId)[0];
    messageDef.updateDateTime = new Date();
    messageDef.description = description;
    messageDef.jsonSchema = jsonSchema;
    return {
      ...state,
      MessageDefinitionLst: [...messageDefLst]
    };
  }),
  on(fromActions.completePublishMessageDefinition, (state, { vpn, applicationDomainId, messageName, newMessageDefId }) => {
    let messageDefLst = JSON.parse(JSON.stringify(state.MessageDefinitionLst)) as MessageDefinitionResult[];
    const lastMessage = messageDefLst.filter(m => m.name === messageName).sort((a, b) => b.version - a.version)[0];
    lastMessage.id = newMessageDefId;
    lastMessage.description = lastMessage.description;
    lastMessage.jsonSchema = lastMessage.jsonSchema;
    lastMessage.version = lastMessage.version + 1;
    lastMessage.createDateTime = new Date();
    lastMessage.updateDateTime = new Date();
    return {
      ...state,
      MessageDefinitionLst: [...messageDefLst]
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

export function getClientReducer(state: ClientState | undefined, action: Action) {
  return clientReducer(state, action);
}

export function getApplicationDomainLstReducer(state: ApplicationDomainLstState | undefined, action: Action) {
  return applicationDomainLstReducer(state, action);
}

export function getMessageDefLstReducer(state: MessageDefinitionLstState | undefined, action: Action) {
  return messageDefLstReducer(state, action);
}

export function getApplicationDomainReducer(state: ApplicationDomainState | undefined, action: Action) {
  return applicationDomainReducer(state, action);
}
