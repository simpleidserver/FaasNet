import { Routes } from '@angular/router';
import { ServerStatusComponent } from './status/status.component';

export const routes: Routes = [
  {
    path: '', redirectTo: 'functions', pathMatch: 'full'
  },
  {
    path: 'status', component: ServerStatusComponent
  },
  {
    path: 'functions',
    loadChildren: async () => (await import('./functions/functions.module')).FunctionsModule
  },
  {
    path: 'statemachines',
    loadChildren: async () => (await import('./statemachines/statemachines.module')).StateMachinesModule
  },
  {
    path: 'statemachineinstances',
    loadChildren: async () => (await import('./statemachineinstances/statemachineinstances.module')).StateMachineInstancesModule
  },
  {
    path: 'vpns',
    loadChildren: async () => (await import('./vpns/vpns.module')).VpnsModule
  },
  {
    path: 'logging/eventmesh',
    loadChildren: async () => (await import('./eventmeshlogs/eventmeshlogging.module')).EventMeshLoggingModule
  }
];
