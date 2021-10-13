import { Action, createReducer, on } from '@ngrx/store';
import { PrometheusQueryRangeResult, PrometheusQueryResult } from '../../common/prometheus-query.model';
import { SearchResult } from '../../common/search.model';
import * as fromActions from '../actions/function.actions';
import { FunctionDetailsResult } from '../models/function-details.model';
import { FunctionResult } from '../models/function.model';

export interface SearchFunctionsState {
  Functions: SearchResult<FunctionResult>;
}

export interface FunctionState {
  Configuration: any | null;
  Function: FunctionResult | null;
  Threads: PrometheusQueryRangeResult | null;
  VirtualMemoryBytes: PrometheusQueryRangeResult | null;
  CpuUsage: PrometheusQueryRangeResult | null;
  RequestDuration: PrometheusQueryRangeResult | null;
  Details: FunctionDetailsResult | null;
  TotalRequests: PrometheusQueryResult | null;
}

export const initialSearchFunctionsState: SearchFunctionsState = {
  Functions: new SearchResult<FunctionResult>()
};

export const initialFunctionState: FunctionState = {
  Configuration: null,
  Function: null,
  Threads: null,
  VirtualMemoryBytes: null,
  CpuUsage: null,
  RequestDuration: null,
  Details: null,
  TotalRequests: null
};

const searchFunctionsReducer = createReducer(
  initialSearchFunctionsState,
  on(fromActions.completeSearch, (state, { content }) => {
    return {
      ...state,
      Functions: { ...content }
    };
  })
);

const functionReducer = createReducer(
  initialFunctionState,
  on(fromActions.completeGetConfiguration, (state, { content }) => {
    return {
      ...state,
      Configuration: { ...content }
    };
  }),
  on(fromActions.completeGet, (state, { content }) => {
    return {
      ...state,
      Function: { ...content }
    };
  }),
  on(fromActions.completeGetThreads, (state, { content }) => {
    return {
      ...state,
      Threads: content
    };
  }),
  on(fromActions.completeGetVirtualMemoryBytes, (state, { content }) => {
    return {
      ...state,
      VirtualMemoryBytes: content
    };
  }),
  on(fromActions.completeGetCpuUsage, (state, { content }) => {
    return {
      ...state,
      CpuUsage: content
    }
  }),
  on(fromActions.completeGetRequestDuration, (state, { content }) => {
    return {
      ...state,
      RequestDuration: content
    }
  }),
  on(fromActions.completeGetDetails, (state, { content }) => {
    return {
      ...state,
      Details: content
    }
  }),
  on(fromActions.completeGetTotalRequests, (state, { content }) => {
    return {
      ...state,
      TotalRequests: content
    }
  })
);

export function getSearchFunctionsReducer(state: SearchFunctionsState | undefined, action: Action) {
  return searchFunctionsReducer(state, action);
}

export function getFunctionReducer(state: FunctionState | undefined, action: Action) {
  return functionReducer(state, action);
}
