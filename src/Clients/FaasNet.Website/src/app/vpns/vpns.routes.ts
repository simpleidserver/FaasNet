import { RouterModule, Routes } from '@angular/router';
import { ListVpnComponent } from './list/list.component';


const routes: Routes = [
  {
    path: '',
    component: ListVpnComponent
  }
];

export const VpnsRoutes = RouterModule.forChild(routes);
