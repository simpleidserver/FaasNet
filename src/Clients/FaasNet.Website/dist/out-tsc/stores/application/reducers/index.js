import { createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/application.actions';
export const initialApplicationDomainsState = {
    ApplicationDomains: []
};
const applicationDomainsReducer = createReducer(initialApplicationDomainsState, on(fromActions.completeGetAllApplicationDomains, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { ApplicationDomains: [...content] });
}), on(fromActions.completeAddApplicationDomain, (state, { content }) => {
    const applicationDomains = [...state.ApplicationDomains];
    applicationDomains.push(content);
    return Object.assign(Object.assign({}, state), { ApplicationDomains: applicationDomains });
}));
export function getApplicationDomainsReducer(state, action) {
    return applicationDomainsReducer(state, action);
}
//# sourceMappingURL=index.js.map