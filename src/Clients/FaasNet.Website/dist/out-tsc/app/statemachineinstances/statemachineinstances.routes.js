import { RouterModule } from '@angular/router';
import { ListStateMachineInstanceComponent } from './list/list.component';
import { ViewStateMachineInstanceComponent } from './view/view.component';
const routes = [
    {
        path: '',
        component: ListStateMachineInstanceComponent
    },
    {
        path: ':id/:instid',
        component: ViewStateMachineInstanceComponent
    }
];
export const StateMachineInstancesRoutes = RouterModule.forChild(routes);
//# sourceMappingURL=statemachineinstances.routes.js.map