import { createAction, props } from '@ngrx/store';
import { SearchResult } from '../../common/search.model';
import { StateMachine } from '../models/statemachine.model';

export const startSearch = createAction('[StateMachines] START_SEARCH_STATE_MACHINES', props<{ order: string, direction: string, count: number, startIndex: number }>());
export const completeSearch = createAction('[StateMachines] COMPLETE_SEARCH_STATE_MACHINES', props<{ content: SearchResult<StateMachine> }>());
export const errorSearch = createAction('[StateMachines] ERROR_SEARCH_STATE_MACHINES');
export const startGetJson = createAction('[StateMachines] START_GET_JSON_STATE_MACHINE', props<{ id: string }>());
export const completeGetJson = createAction('[StateMachines] COMPLETE_GET_JSON_STATE_MACHINE', props <{ content: any }>());
export const errorGetJson = createAction('[StateMachines] ERROR_GET_JSON_STATE_MACHINE');
export const startAddEmpty = createAction('[StateMachines] START_ADD_EMPTY_STATE_MACHINE', props<{ name: string, description: string }>());
export const completeAddEmpty = createAction('[StateMachines] COMPLETE_ADD_EMPTY_STATE_MACHINE', props<{ id: string }>());
export const errorAddEmpty = createAction('[StateMachines] ERROR_ADD_EMPTY_STATE_MACHINE');
export const startUpdate = createAction('[StateMachines] START_UPDATE_STATE_MACHINE', props<{ stateMachine: any }>());
export const completeUpdate = createAction('[StateMachines] COMPLETE_UPDATE_STATE_MACHINE');
export const errorUpdate = createAction('[StateMachines] ERROR_UPDATE_STATE_MACHINE');


