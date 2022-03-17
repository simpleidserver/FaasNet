import { createAction, props } from '@ngrx/store';
export const startSearch = createAction('[StateMachineInstances] START_SEARCH_STATEMACHINE_INSTANCES', props());
export const completeSearch = createAction('[StateMachineInstances] COMPLETE_SEARCH_STATEMACHINE_INSTANCES', props());
export const errorSearch = createAction('[StateMachineInstances] ERROR_SEARCH_STATEMACHINE_INSTANCES');
export const startGet = createAction('[StateMachineInstances] START_GET_STATEMACHINE_INSTANCE', props());
export const completeGet = createAction('[StateMachineInstances] COMPLETE_GET_STATEMACHINE_INSTANCE', props());
export const errorGet = createAction('[StateMachineInstances] ERROR_GET_STATEMACHINE_INSTANCE');
//# sourceMappingURL=statemachineinstances.actions.js.map