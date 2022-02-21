import { RouterModule, Routes } from '@angular/router';
import { ListEventMeshServersComponent } from './list/list.component';


const routes: Routes = [
  {
    path: '',
    component: ListEventMeshServersComponent
  }
];

export const EventMeshServersRoutes = RouterModule.forChild(routes);
