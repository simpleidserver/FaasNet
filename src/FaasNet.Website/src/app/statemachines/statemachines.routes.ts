import { RouterModule, Routes } from '@angular/router';
import { EditStateMachineComponent } from './edit/edit.component';
import { ListStateMachinesComponent } from './list/list.component';


const routes: Routes = [
  {
    path: ':id',
    component: EditStateMachineComponent
  },
  {
    path: '',
    component: ListStateMachinesComponent
  }
];

export const StateMachinesRoutes = RouterModule.forChild(routes);
