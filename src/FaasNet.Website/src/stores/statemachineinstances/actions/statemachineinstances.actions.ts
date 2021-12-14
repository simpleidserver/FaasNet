import { createAction, props } from '@ngrx/store';
import { SearchResult } from '../../common/search.model';
import { StateMachineInstanceDetails } from '../models/statemachineinstance-details.model';
import { StateMachineInstance } from '../models/statemachineinstance.model';

export const startSearch = createAction('[StateMachineInstances] START_SEARCH_STATEMACHINE_INSTANCES', props<{ order: string, direction: string, count: number, startIndex: number }>());
export const completeSearch = createAction('[StateMachineInstances] COMPLETE_SEARCH_STATEMACHINE_INSTANCES', props<{ content: SearchResult<StateMachineInstance> }>());
export const errorSearch = createAction('[StateMachineInstances] ERROR_SEARCH_STATEMACHINE_INSTANCES');
export const startGet = createAction('[StateMachineInstances] START_GET_STATEMACHINE_INSTANCE', props<{ id: string  }>());
export const completeGet = createAction('[StateMachineInstances] COMPLETE_GET_STATEMACHINE_INSTANCE', props<{ content: StateMachineInstanceDetails }>());
export const errorGet = createAction('[StateMachineInstances] ERROR_GET_STATEMACHINE_INSTANCE');


