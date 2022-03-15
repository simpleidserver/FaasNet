import { Action, createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/application.actions';
import { ApplicationDomainResult } from '../models/applicationdomain.model';

export interface ApplicationDomainsState {
  ApplicationDomains: ApplicationDomainResult[];
}

export const initialApplicationDomainsState: ApplicationDomainsState = {
  ApplicationDomains: []
};

const applicationDomainsReducer = createReducer(
  initialApplicationDomainsState,
  on(fromActions.completeGetAllApplicationDomains, (state, { content }) => {
    return {
      ...state,
      ApplicationDomains: [ ...content ]
    };
  }),
  on(fromActions.completeAddApplicationDomain, (state, { content }) => {
    const applicationDomains = [...state.ApplicationDomains];
    applicationDomains.push(content);
    return {
      ...state,
      ApplicationDomains: applicationDomains
    };
  })
);

export function getApplicationDomainsReducer(state: ApplicationDomainsState | undefined, action: Action) {
  return applicationDomainsReducer(state, action);
}
