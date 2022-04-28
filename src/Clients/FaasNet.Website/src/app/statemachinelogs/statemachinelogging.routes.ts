import { RouterModule, Routes } from '@angular/router';
import { StateMachineLoggingComponent } from './statemachinelogging.component';
const routes: Routes = [
  {
    path: '',
    component: StateMachineLoggingComponent
  }
];

export const StateMachineLoggingRoutes = RouterModule.forChild(routes);
