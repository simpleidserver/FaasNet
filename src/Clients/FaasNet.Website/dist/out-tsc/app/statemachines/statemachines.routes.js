import { RouterModule } from '@angular/router';
import { EditStateMachineComponent } from './edit/edit.component';
import { ListStateMachinesComponent } from './list/list.component';
const routes = [
    {
        path: ':id/:action',
        component: EditStateMachineComponent
    },
    {
        path: '',
        component: ListStateMachinesComponent
    }
];
export const StateMachinesRoutes = RouterModule.forChild(routes);
//# sourceMappingURL=statemachines.routes.js.map