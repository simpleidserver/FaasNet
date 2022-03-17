import { ServerStatusComponent } from './status/status.component';
export const routes = [
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
    }
];
//# sourceMappingURL=app.routes.js.map