import { RouterModule, Routes } from '@angular/router';
import { ListStateMachineInstanceComponent } from './list/list.component';
import { ViewStateMachineInstanceComponent } from './view/view.component';


const routes: Routes = [
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
