import { RouterModule, Routes } from '@angular/router';
import { ListFunctionsComponent } from './list/list.component';
import { InfoFunctionComponent } from './view/info/info.component';
import { InvokeFunctionComponent } from './view/invoke/invoke.component';
import { ViewFunctionComponent } from './view/view.component';


const routes: Routes = [
  {
    path: '',
    component: ListFunctionsComponent
  },
  {
    path: ':name',
    component: ViewFunctionComponent,
    children: [
      {
        path: '', redirectTo: 'info', pathMatch: 'full'
      },
      {
        path: 'info',
        component: InfoFunctionComponent
      },
      {
        path: 'invoke',
        component: InvokeFunctionComponent
      }
    ]
  }
];

export const FunctionsRoutes = RouterModule.forChild(routes);
