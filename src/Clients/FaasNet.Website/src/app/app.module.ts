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
import { appReducer } from '../stores/appstate';
import { AsyncApiService } from '../stores/asyncapi/services/asyncapi.service';
import { EventMeshServerEffects } from '../stores/eventmeshservers/effects/eventmeshserver.effects';
import { EventMeshServerService } from '../stores/eventmeshservers/services/eventmeshserver.service';
import { FunctionEffects } from '../stores/functions/effects/function.effects';
import { FunctionService } from '../stores/functions/services/function.service';
import { OpenApiService } from '../stores/openapi/services/openapi.service';
import { StateMachineInstancesEffects } from '../stores/statemachineinstances/effects/statemachineinstances.effects';
import { StateMachineInstancesService } from '../stores/statemachineinstances/services/statemachineinstances.service';
import { StateMachineEffects } from '../stores/statemachines/effects/statemachines.effects';
import { StateMachinesService } from '../stores/statemachines/services/statemachines.service';
import { AppComponent } from './app.component';
import { routes } from './app.routes';
import { MaterialModule } from './shared/material.module';

export function createTranslateLoader(http: HttpClient) {
  let url = environment.baseUrl + 'assets/i18n/';
  return new TranslateHttpLoader(http, url, '.json');
}


@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    RouterModule.forRoot(routes),
    MonacoEditorModule.forRoot(),
    MaterialModule,
    BrowserModule,
    HttpClientModule,
    BrowserAnimationsModule,
    EffectsModule.forRoot([FunctionEffects, StateMachineEffects, StateMachineInstancesEffects, EventMeshServerEffects]),
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
  providers: [FunctionService, StateMachinesService, StateMachineInstancesService, OpenApiService, AsyncApiService, MatPanelService, EventMeshServerService],
  bootstrap: [AppComponent]
})
export class AppModule { }
