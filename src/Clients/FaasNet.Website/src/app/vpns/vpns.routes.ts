import { RouterModule, Routes } from '@angular/router';
import { ListVpnComponent } from './list/list.component';
import { AppDomainsVpnComponent } from './view/appdomains/appdomains.component';
import { EditorDomainComponent } from './view/appdomains/view/editor/editor.component';
import { AppDomainMessagesEditorComponent } from './view/appdomains/view/messages/messageseditor.component';
import { ViewVpnAppDomainComponent } from './view/appdomains/view/view.component';
import { ClientsVpnComponent } from './view/clients/clients.component';
import { ViewVpnClientSessionsComponent } from './view/clients/view/sessions/sessions.component';
import { ViewVpnClientComponent } from './view/clients/view/view.component';
import { InfoVpnComponent } from './view/info/info.component';
import { ViewVpnComponent } from './view/view.component';


const routes: Routes = [
  {
    path: '',
    component: ListVpnComponent
  },
  {
    path: ':vpnName/clients/:clientId',
    component: ViewVpnClientComponent,
    children: [
      {
        path: '', redirectTo: 'sessions', pathMatch: 'full'
      },
      {
        path: 'sessions',
        component: ViewVpnClientSessionsComponent
      }
    ]
  },
  {
    path: ':vpnName/appdomains/:appDomainId',
    component: ViewVpnAppDomainComponent,
    children: [
      {
        path: '', redirectTo: 'messages', pathMatch: 'full'
      },
      {
        path: 'messages',
        component: AppDomainMessagesEditorComponent
      },
      {
        path: 'editor',
        component: EditorDomainComponent
      }
    ]
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
      },
      {
        path: 'appdomains',
        component: AppDomainsVpnComponent
      }
    ]
  }
];

export const VpnsRoutes = RouterModule.forChild(routes);
