import { __decorate } from "tslib";
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { environment } from '@envs/environment';
import { EffectsModule } from '@ngrx/effects';
import { StoreModule } from '@ngrx/store';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { MatPanelService } from '../components/matpanel/matpanelservice';
import { MonacoEditorModule } from '../components/monaco-editor/editor.module';
import { ApplicationEffects } from '../stores/application/effects/application.effects';
import { ApplicationService } from '../stores/application/services/eventmeshserver.service';
import { appReducer } from '../stores/appstate';
import { AsyncApiService } from '../stores/asyncapi/services/asyncapi.service';
import { EventMeshServerEffects } from '../stores/eventmeshservers/effects/eventmeshserver.effects';
import { EventMeshServerService } from '../stores/eventmeshservers/services/eventmeshserver.service';
import { FunctionEffects } from '../stores/functions/effects/function.effects';
import { FunctionService } from '../stores/functions/services/function.service';
import { OpenApiService } from '../stores/openapi/services/openapi.service';
import { ServerEffects } from '../stores/server/effects/server.effects';
import { ServerService } from '../stores/server/services/server.service';
import { StateMachineInstancesEffects } from '../stores/statemachineinstances/effects/statemachineinstances.effects';
import { StateMachineInstancesService } from '../stores/statemachineinstances/services/statemachineinstances.service';
import { StateMachineEffects } from '../stores/statemachines/effects/statemachines.effects';
import { StateMachinesService } from '../stores/statemachines/services/statemachines.service';
import { VpnEffects } from '../stores/vpn/effects/vpn.effects';
import { VpnService } from '../stores/vpn/services/vpn.service';
import { AppComponent } from './app.component';
import { routes } from './app.routes';
import { MaterialModule } from './shared/material.module';
import { ServerStatusComponent } from './status/status.component';
export function createTranslateLoader(http) {
    let url = environment.baseUrl + 'assets/i18n/';
    return new TranslateHttpLoader(http, url, '.json');
}
let AppModule = class AppModule {
};
AppModule = __decorate([
    NgModule({
        declarations: [
            AppComponent,
            ServerStatusComponent
        ],
        imports: [
            RouterModule.forRoot(routes),
            MonacoEditorModule.forRoot(),
            MaterialModule,
            BrowserModule,
            HttpClientModule,
            BrowserAnimationsModule,
            EffectsModule.forRoot([FunctionEffects, StateMachineEffects, StateMachineInstancesEffects, EventMeshServerEffects, ApplicationEffects, ServerEffects, VpnEffects]),
            StoreModule.forRoot(appReducer),
            TranslateModule.forRoot({
                loader: {
                    provide: TranslateLoader,
                    useFactory: createTranslateLoader,
                    deps: [HttpClient]
                }
            }),
            StoreDevtoolsModule.instrument({
                maxAge: 10
            })
        ],
        providers: [FunctionService, StateMachinesService, StateMachineInstancesService, OpenApiService, AsyncApiService, MatPanelService, EventMeshServerService, ApplicationService, ServerService, VpnService],
        bootstrap: [AppComponent]
    })
], AppModule);
export { AppModule };
//# sourceMappingURL=app.module.js.map