import { RouterModule, Routes } from '@angular/router';
import { EditDomainComponent } from './edit/edit.component';


const routes: Routes = [
  {
    path: '',
    component: EditDomainComponent
  }
];

export const DomainsRoutes = RouterModule.forChild(routes);
