import { createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/vpn.actions';
import { VpnResult } from '../models/vpn.model';
export const initialVpnLstState = {
    VpnLst: []
};
const vpnLstReducer = createReducer(initialVpnLstState, on(fromActions.completeGetAllVpn, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { VpnLst: [...content] });
}), on(fromActions.completeAddVpn, (state, { name, description }) => {
    const result = new VpnResult();
    result.name = name;
    result.description = description;
    result.createDateTime = new Date();
    result.updateDateTime = new Date();
    const vpnLst = JSON.parse(JSON.stringify(state.VpnLst));
    vpnLst.push(result);
    return Object.assign(Object.assign({}, state), { VpnLst: [...vpnLst] });
}), on(fromActions.completeDeleteVpn, (state, { name }) => {
    let vpnLst = JSON.parse(JSON.stringify(state.VpnLst));
    vpnLst = vpnLst.filter(v => v.name !== name);
    return Object.assign(Object.assign({}, state), { VpnLst: [...vpnLst] });
}));
export function getVpnLstReducer(state, action) {
    return vpnLstReducer(state, action);
}
//# sourceMappingURL=index.js.map