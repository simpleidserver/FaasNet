import { Action, createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/messagedefs.actions';
import { MessageDefinitionResult } from '../models/messagedefinition.model';

export interface MessageDefinitionLstState {
  MessageDefinitionLst: MessageDefinitionResult[];
}

export interface MessageDefinitionState {
  MessageDefinition: MessageDefinitionResult | null;
}

export const initialMessageDefinitionLstState: MessageDefinitionLstState = {
  MessageDefinitionLst: []
};

export const initialMessageDefinitionState: MessageDefinitionState = {
  MessageDefinition : null
};

const messageDefLstReducer = createReducer(
  initialMessageDefinitionLstState,
  on(fromActions.completeGetLatestMessages, (state, { content }) => {
    return {
      ...state,
      MessageDefinitionLst: [...content]
    };
  }),
  on(fromActions.completeAddMessageDefinition, (state, { id, appDomainId, name, description, jsonSchema }) => {
    const messageDefLst = JSON.parse(JSON.stringify(state.MessageDefinitionLst)) as MessageDefinitionResult[];
    var record = new MessageDefinitionResult();
    record.id = id;
    record.name = name;
    record.description = description;
    record.version = 0;
    record.jsonSchema = jsonSchema;
    record.createDateTime = new Date();
    record.updateDateTime = new Date();
    messageDefLst.push(record);
    return {
      ...state,
      MessageDefinitionLst: [...messageDefLst]
    };
  }),
  on(fromActions.completeUpdateMessageDefinition, (state, { id, description, jsonSchema }) => {
    let messageDefLst = JSON.parse(JSON.stringify(state.MessageDefinitionLst)) as MessageDefinitionResult[];
    const messageDef = messageDefLst.filter(m => m.id === id)[0];
    messageDef.updateDateTime = new Date();
    messageDef.description = description;
    messageDef.jsonSchema = jsonSchema;
    return {
      ...state,
      MessageDefinitionLst: [...messageDefLst]
    };
  }),
  on(fromActions.completePublishMessageDefinition, (state, { id, newId }) => {
    let messageDefLst = JSON.parse(JSON.stringify(state.MessageDefinitionLst)) as MessageDefinitionResult[];
    const lastMessage = messageDefLst.filter(m => m.id === id)[0];
    lastMessage.id = newId;
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

export function getMessageDefLstReducer(state: MessageDefinitionLstState | undefined, action: Action) {
  return messageDefLstReducer(state, action);
}
