import { createAction, props } from '@ngrx/store';
import { SearchResult } from '../../common/search.model';
import { StateMachineInstance } from '../models/statemachineinstance.model';

export const startSearch = createAction('[StateMachineInstances] START_SEARCH_STATEMACHINE_INSTANCES', props<{ order: string, direction: string, count: number, startIndex: number }>());
export const completeSearch = createAction('[StateMachineInstances] COMPLETE_SEARCH_STATEMACHINE_INSTANCES', props<{ content: SearchResult<StateMachineInstance> }>());
export const errorSearch = createAction('[StateMachineInstances] ERROR_SEARCH_STATEMACHINE_INSTANCES');


