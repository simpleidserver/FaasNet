import { RouterModule, Routes } from '@angular/router';
import { ListStateMachineInstanceComponent } from './list/list.component';


const routes: Routes = [
  {
    path: '',
    component: ListStateMachineInstanceComponent
  }
];

export const StateMachineInstancesRoutes = RouterModule.forChild(routes);
