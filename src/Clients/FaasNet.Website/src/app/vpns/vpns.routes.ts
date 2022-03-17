import { RouterModule, Routes } from '@angular/router';
import { ListVpnComponent } from './list/list.component';
import { ClientsVpnComponent } from './view/clients/clients.component';
import { InfoVpnComponent } from './view/info/info.component';
import { ViewVpnComponent } from './view/view.component';


const routes: Routes = [
  {
    path: '',
    component: ListVpnComponent
  },
  {
    path: ':name',
    component: ViewVpnComponent,
    children: [
      {
        path: '', redirectTo: 'info', pathMatch: 'full'
      },
      {
        path: 'info',
        component: InfoVpnComponent
      },
      {
        path: 'clients',
        component: ClientsVpnComponent
      }
    ]
  }
];

export const VpnsRoutes = RouterModule.forChild(routes);
