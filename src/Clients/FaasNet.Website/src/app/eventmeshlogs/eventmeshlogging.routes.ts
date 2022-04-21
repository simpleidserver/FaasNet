import { RouterModule, Routes } from '@angular/router';
import { EventMeshLoggingComponent } from './eventmeshlogging.component';
const routes: Routes = [
  {
    path: '',
    component: EventMeshLoggingComponent
  }
];

export const EventMeshLoggingRoutes = RouterModule.forChild(routes);
