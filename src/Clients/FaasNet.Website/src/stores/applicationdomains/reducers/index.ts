import { Action, createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/applicationdomains.actions';
import { AppDomainResult } from '../models/appdomain.model';

export interface ApplicationDomainsState {
  ApplicationDomains: AppDomainResult[];
}

export interface ApplicationDomainState {
  ApplicationDomain: AppDomainResult | null;
}


export const initialApplicationDomainsState: ApplicationDomainsState = {
  ApplicationDomains: []
};

export const initialApplicationDomainState: ApplicationDomainState = {
  ApplicationDomain: null
};


const applicationDomainsReducer = createReducer(
  initialApplicationDomainsState,
  on(fromActions.completeGetAllAppDomains, (state, { content }) => {
    return {
      ...state,
      ApplicationDomains: [ ...content ]
    };
  }),
  on(fromActions.completeAddAppDomain, (state, { id, name, description, rootTopic }) => {
    const appDomainLst = JSON.parse(JSON.stringify(state.ApplicationDomains)) as AppDomainResult[];
    const record = new AppDomainResult();
    record.id = id;
    record.name = name;
    record.description = description;
    record.rootTopic = rootTopic;
    record.createDateTime = new Date();
    record.updateDateTime = new Date();
    appDomainLst.push(record);
    return {
      ...state,
      ApplicationDomains: appDomainLst
    };
  }),
  on(fromActions.completeDeleteAppDomain, (state, { id }) => {
    let appDomainLst = JSON.parse(JSON.stringify(state.ApplicationDomains)) as AppDomainResult[];
    appDomainLst = appDomainLst.filter(c => c.id !== id);
    return {
      ...state,
      ApplicationDomains: appDomainLst
    };
  })
);

const applicationDomainReducer = createReducer(
  initialApplicationDomainState,
  on(fromActions.completeGetAppDomain, (state, { content }) => {
    return {
      ...state,
      ApplicationDomain: { ...content }
    };
  })
);


export function getApplicationDomainsReducer(state: ApplicationDomainsState | undefined, action: Action) {
  return applicationDomainsReducer(state, action);
}

export function getApplicationDomainReducer(state: ApplicationDomainState | undefined, action: Action) {
  return applicationDomainReducer(state, action);
}
