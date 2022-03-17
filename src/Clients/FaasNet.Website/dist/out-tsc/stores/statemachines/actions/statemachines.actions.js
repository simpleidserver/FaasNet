import { createAction, props } from '@ngrx/store';
export const startSearch = createAction('[StateMachines] START_SEARCH_STATE_MACHINES', props());
export const completeSearch = createAction('[StateMachines] COMPLETE_SEARCH_STATE_MACHINES', props());
export const errorSearch = createAction('[StateMachines] ERROR_SEARCH_STATE_MACHINES');
export const startGetJson = createAction('[StateMachines] START_GET_JSON_STATE_MACHINE', props());
export const completeGetJson = createAction('[StateMachines] COMPLETE_GET_JSON_STATE_MACHINE', props());
export const errorGetJson = createAction('[StateMachines] ERROR_GET_JSON_STATE_MACHINE');
export const startAddEmpty = createAction('[StateMachines] START_ADD_EMPTY_STATE_MACHINE', props());
export const completeAddEmpty = createAction('[StateMachines] COMPLETE_ADD_EMPTY_STATE_MACHINE', props());
export const errorAddEmpty = createAction('[StateMachines] ERROR_ADD_EMPTY_STATE_MACHINE');
export const startUpdate = createAction('[StateMachines] START_UPDATE_STATE_MACHINE', props());
export const completeUpdate = createAction('[StateMachines] COMPLETE_UPDATE_STATE_MACHINE', props());
export const errorUpdate = createAction('[StateMachines] ERROR_UPDATE_STATE_MACHINE');
export const startLaunch = createAction('[StateMachines] START_LAUNCH_STATE_MACHINE', props());
export const completeLaunch = createAction('[StateMachines] COMPLETE_LAUNCH_STATE_MACHINE', props());
export const errorLaunch = createAction('[StateMachines] ERROR_LAUNCH_STATE_MACHINE');
//# sourceMappingURL=statemachines.actions.js.map