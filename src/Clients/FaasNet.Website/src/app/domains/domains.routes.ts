import { RouterModule, Routes } from '@angular/router';
import { EditDomainComponent } from './edit/edit.component';
import { EditorDomainComponent } from './edit/editor/editor.component';
import { MessagesDomainComponent } from './edit/messages/messages.component';
import { ListApplicationDomainComponent } from './list/list.component';


const routes: Routes = [
  {
    path: '',
    component: ListApplicationDomainComponent
  },
  {
    path: ':id',
    component: EditDomainComponent,
    children: [
      { path: 'messages', component: MessagesDomainComponent },
      { path: 'editor', component: EditorDomainComponent }
    ]
  }
];

export const DomainsRoutes = RouterModule.forChild(routes);
