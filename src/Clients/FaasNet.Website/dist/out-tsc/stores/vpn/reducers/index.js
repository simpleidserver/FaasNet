import { createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/vpn.actions';
import { AppDomainResult } from '../models/appdomain.model';
import { ClientResult } from '../models/client.model';
import { MessageDefinitionResult } from '../models/messagedefinition.model';
import { VpnResult } from '../models/vpn.model';
export const initialVpnLstState = {
    VpnLst: []
};
export const initialClientLstState = {
    ClientLst: []
};
export const initialClientState = {
    Client: null
};
export const initialVpnState = {
    Vpn: null
};
export const initialMessageDefinitionLstState = {
    MessageDefinitionLst: []
};
export const initialMessageDefinitionState = {
    MessageDefinition: null
};
export const initialApplicationDomainLstState = {
    ApplicationDomainLst: []
};
export const initialApplicationDomainState = {
    ApplicationDomain: null
};
const vpnLstReducer = createReducer(initialVpnLstState, on(fromActions.completeGetAllVpn, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { VpnLst: [...content] });
}), on(fromActions.completeAddVpn, (state, { name, description }) => {
    const result = new VpnResult();
    result.name = name;
    result.description = description;
    result.createDateTime = new Date();
    result.updateDateTime = new Date();
    const vpnLst = JSON.parse(JSON.stringify(state.VpnLst));
    vpnLst.push(result);
    return Object.assign(Object.assign({}, state), { VpnLst: [...vpnLst] });
}), on(fromActions.completeDeleteVpn, (state, { name }) => {
    let vpnLst = JSON.parse(JSON.stringify(state.VpnLst));
    vpnLst = vpnLst.filter(v => v.name !== name);
    return Object.assign(Object.assign({}, state), { VpnLst: [...vpnLst] });
}));
const vpnReducer = createReducer(initialVpnState, on(fromActions.completeGetVpn, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { Vpn: Object.assign({}, content) });
}));
const clientLstReducer = createReducer(initialClientLstState, on(fromActions.completeGetAllClients, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { ClientLst: [...content] });
}), on(fromActions.completeAddClient, (state, { name, clientId, purposes }) => {
    const clientLst = JSON.parse(JSON.stringify(state.ClientLst));
    var record = new ClientResult();
    record.clientId = clientId;
    record.purposes = purposes;
    record.createDateTime = new Date();
    clientLst.push(record);
    return Object.assign(Object.assign({}, state), { ClientLst: [...clientLst] });
}), on(fromActions.completeDeleteClient, (state, { name, clientId }) => {
    let clientLst = JSON.parse(JSON.stringify(state.ClientLst));
    clientLst = clientLst.filter(c => c.clientId !== clientId);
    return Object.assign(Object.assign({}, state), { ClientLst: [...clientLst] });
}));
const clientReducer = createReducer(initialClientState, on(fromActions.completeGetClient, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { Client: Object.assign({}, content) });
}));
const applicationDomainLstReducer = createReducer(initialApplicationDomainLstState, on(fromActions.completeGetAppDomains, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { ApplicationDomainLst: [...content] });
}), on(fromActions.completeAddAppDomain, (state, { id, name, description, rootTopic }) => {
    const appDomainLst = JSON.parse(JSON.stringify(state.ApplicationDomainLst));
    var record = new AppDomainResult();
    record.id = id;
    record.name = name;
    record.description = description;
    record.rootTopic = rootTopic;
    record.createDateTime = new Date();
    record.updateDateTime = new Date();
    appDomainLst.push(record);
    return Object.assign(Object.assign({}, state), { ApplicationDomainLst: [...appDomainLst] });
}), on(fromActions.completeDeleteAppDomain, (state, { name, appDomainId }) => {
    let appDomainLst = JSON.parse(JSON.stringify(state.ApplicationDomainLst));
    appDomainLst = appDomainLst.filter(c => c.id !== appDomainId);
    return Object.assign(Object.assign({}, state), { ApplicationDomainLst: [...appDomainLst] });
}));
const applicationDomainReducer = createReducer(initialApplicationDomainState, on(fromActions.completeGetAppDomain, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { ApplicationDomain: Object.assign({}, content) });
}));
const messageDefLstReducer = createReducer(initialMessageDefinitionLstState, on(fromActions.completeGetLatestMessages, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { MessageDefinitionLst: [...content] });
}), on(fromActions.completeAddMessageDefinition, (state, { vpn, messageDefId, applicationDomainId, name, description, jsonSchema }) => {
    const messageDefLst = JSON.parse(JSON.stringify(state.MessageDefinitionLst));
    var record = new MessageDefinitionResult();
    record.id = messageDefId;
    record.name = name;
    record.description = description;
    record.version = 0;
    record.createDateTime = new Date();
    record.updateDateTime = new Date();
    messageDefLst.push(record);
    return Object.assign(Object.assign({}, state), { MessageDefinitionLst: [...messageDefLst] });
}), on(fromActions.completeUpdateMessageDefinition, (state, { vpn, applicationDomainId, messageDefId, description, jsonSchema }) => {
    let messageDefLst = JSON.parse(JSON.stringify(state.MessageDefinitionLst));
    const messageDef = messageDefLst.filter(m => m.id !== messageDefId)[0];
    messageDef.updateDateTime = new Date();
    messageDef.description = description;
    messageDef.jsonSchema = jsonSchema;
    return Object.assign(Object.assign({}, state), { MessageDefinitionLst: [...messageDefLst] });
}), on(fromActions.completePublishMessageDefinition, (state, { vpn, applicationDomainId, messageName, newMessageDefId }) => {
    let messageDefLst = JSON.parse(JSON.stringify(state.MessageDefinitionLst));
    const lastMessage = messageDefLst.filter(m => m.name === messageName).sort((a, b) => b.version - a.version)[0];
    let record = new MessageDefinitionResult();
    record.id = newMessageDefId;
    record.name = messageName;
    record.description = lastMessage.description;
    record.jsonSchema = lastMessage.jsonSchema;
    record.version = lastMessage.version + 1;
    record.createDateTime = new Date();
    record.updateDateTime = new Date();
    messageDefLst.push(record);
    return Object.assign(Object.assign({}, state), { MessageDefinitionLst: [...messageDefLst] });
}));
export function getVpnLstReducer(state, action) {
    return vpnLstReducer(state, action);
}
export function getVpnReducer(state, action) {
    return vpnReducer(state, action);
}
export function getClientLstReducer(state, action) {
    return clientLstReducer(state, action);
}
export function getClientReducer(state, action) {
    return clientReducer(state, action);
}
export function getApplicationDomainLstReducer(state, action) {
    return applicationDomainLstReducer(state, action);
}
export function getMessageDefLstReducer(state, action) {
    return messageDefLstReducer(state, action);
}
export function getApplicationDomainReducer(state, action) {
    return applicationDomainReducer(state, action);
}
//# sourceMappingURL=index.js.map