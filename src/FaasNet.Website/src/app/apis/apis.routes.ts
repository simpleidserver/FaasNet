import { RouterModule, Routes } from '@angular/router';
import { EditApiComponent } from './edit/edit.component';


const routes: Routes = [
  {
    path: ':name',
    component: EditApiComponent
  }
];

export const ApisRoutes = RouterModule.forChild(routes);
