import { RouterModule, Routes } from '@angular/router';
import { EditStateMachineComponent } from './edit/edit.component';


const routes: Routes = [
  {
    path: ':name',
    component: EditStateMachineComponent
  }
];

export const StateMachinesRoutes = RouterModule.forChild(routes);
