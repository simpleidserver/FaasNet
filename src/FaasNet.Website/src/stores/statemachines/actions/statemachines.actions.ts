import { createAction, props } from '@ngrx/store';
import { SearchResult } from '../../common/search.model';
import { StateMachine } from '../models/statemachine.model';

export const startSearch = createAction('[StateMachines] START_SEARCH_STATE_MACHINES', props<{ order: string, direction: string, count: number, startIndex: number }>());
export const completeSearch = createAction('[StateMachines] COMPLETE_SEARCH_STATE_MACHINES', props<{ content: SearchResult<StateMachine> }>());
export const errorSearch = createAction('[StateMachines] ERROR_SEARCH_STATE_MACHINES');
export const startGetJson = createAction('[StateMachines] START_GET_JSON_STATE_MACHINE', props<{ id: string }>());
export const completeGetJson = createAction('[StateMachines] COMPLETE_GET_JSON_STATE_MACHINE', props <{ content: any }>());
export const errorGetJson = createAction('[StateMachines] ERROR_GET_JSON_STATE_MACHINE');

