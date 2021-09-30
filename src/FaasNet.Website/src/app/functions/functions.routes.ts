import { RouterModule, Routes } from '@angular/router';
import { ListFunctionsComponent } from './list/list.component';
import { ViewFunctionComponent } from './view/view.component';


const routes: Routes = [
  {
    path: '',
    component: ListFunctionsComponent
  },
  {
    path: ':name',
    component: ViewFunctionComponent
  }
];

export const FunctionsRoutes = RouterModule.forChild(routes);
