import { createAction, props } from '@ngrx/store';
import { SearchResult } from '../../common/search.model';
import { StateMachineInstanceDetails } from '../models/statemachineinstance-details.model';
import { StateMachineInstance } from '../models/statemachineinstance.model';

export const startSearch = createAction('[StateMachineInstances] START_SEARCH_STATEMACHINE_INSTANCES', props<{ order: string, direction: string, count: number, startIndex: number, vpn : string }>());
export const completeSearch = createAction('[StateMachineInstances] COMPLETE_SEARCH_STATEMACHINE_INSTANCES', props<{ content: SearchResult<StateMachineInstance> }>());
export const errorSearch = createAction('[StateMachineInstances] ERROR_SEARCH_STATEMACHINE_INSTANCES');
export const startGet = createAction('[StateMachineInstances] START_GET_STATEMACHINE_INSTANCE', props<{ id: string  }>());
export const completeGet = createAction('[StateMachineInstances] COMPLETE_GET_STATEMACHINE_INSTANCE', props<{ content: StateMachineInstanceDetails }>());
export const errorGet = createAction('[StateMachineInstances] ERROR_GET_STATEMACHINE_INSTANCE');
export const startReactivate = createAction('[StateMachineInstances] START_REACTIVATE_INSTANCE', props<{ id: string }>());
export const completeReactivate = createAction('[StateMachineInstances] COMPLETE_REACTIVATE_INSTANCE');
export const errorReactivate = createAction('[StateMachineInstances] ERROR_REACTIVATE_INSTANCE');
