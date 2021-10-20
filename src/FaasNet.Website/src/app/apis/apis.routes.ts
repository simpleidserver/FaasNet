import { RouterModule, Routes } from '@angular/router';
import { EditApiComponent } from './edit/edit.component';
import { ListApiDefComponent } from './list/list.component';
import { InfoApiComponent } from './view/info/info.component';
import { OperationsApiComponents } from './view/operations/operations.component';
import { ViewApiDefComponent } from './view/view.component';


const routes: Routes = [
  {
    path: '',
    component: ListApiDefComponent
  },
  {
    path: ':name',
    component: ViewApiDefComponent,
    children: [
      {
        path: '',
        redirectTo: 'info',
        pathMatch: 'full'
      },
      {
        path: 'info',
        component: InfoApiComponent
      },
      {
        path: 'operations',
        component: OperationsApiComponents
      }
    ]
  },
  {
    path: ':name/operations/:opname',
    component: EditApiComponent
  }
];

export const ApisRoutes = RouterModule.forChild(routes);
