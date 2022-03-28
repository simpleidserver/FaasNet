import { createAction, props } from '@ngrx/store';
import { MessageDefinitionResult } from '../models/messagedefinition.model';

export const getLatestMessages = createAction('[MESSAGEDEFS] GET_LATEST_MESSAGES', props<{ appDomainId: string }>());
export const completeGetLatestMessages = createAction('[MESSAGEDEFS] COMPLETE_GET_LATEST_MESSAGES', props<{ content: MessageDefinitionResult[] }>());
export const errorGetLatestMessages = createAction('[MESSAGEDEFS] ERROR_GET_LATEST_MESSAGES');
export const addMessageDefinition = createAction('[MESSAGEDEFS] ADD_MESSAGE_DEFINITION', props<{ appDomainId: string, name: string, description: string, jsonSchema: string }>());
export const completeAddMessageDefinition = createAction('[MESSAGEDEFS] COMPLETE_ADD_MESSAGE_DEFINITION', props<{ id: string, appDomainId: string, name: string, description: string, jsonSchema: string }>());
export const errorAddMessageDefinition = createAction('[MESSAGEDEFS] ERROR_ADD_MESSAGE_DEFINITION');
export const updateMessageDefinition = createAction('[MESSAGEDEFS] UPDATE_MESSAGE_DEFINITION', props<{ id: string, description: string, jsonSchema: string }>());
export const completeUpdateMessageDefinition = createAction('[MESSAGEDEFS] COMPLETE_UPDATE_MESSAGE_DEFINITION', props<{ id: string, description: string, jsonSchema: string }>());
export const errorUpdateMessageDefinition = createAction('[MESSAGEDEFS] ERROR_UPDATE_MESSAGE_DEFINITION');
export const publishMessageDefinition = createAction('[MESSAGEDEFS] PUBLISH_MESSAGE_DEFINITION', props<{ id: string, messageName: string }>());
export const completePublishMessageDefinition = createAction('[MESSAGEDEFS] COMPLETE_PUBLISH_MESSAGE_DEFINITION', props<{ id: string, newId: string }>());
export const errorPublishMessageDefinition = createAction('[MESSAGEDEFS] ERROR_PUBLISH_MESSAGE_DEFINITION');
