import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '', redirectTo: 'functions', pathMatch: 'full'
  },
  {
    path: 'functions',
    loadChildren: async () => (await import('./functions/functions.module')).FunctionsModule
  },
  {
    path: 'apis',
    loadChildren: async () => (await import('./apis/apis.module')).ApisModule
  }
];
