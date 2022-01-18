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
import { appReducer } from '../stores/appstate';
import { FunctionEffects } from '../stores/functions/effects/function.effects';
import { FunctionService } from '../stores/functions/services/function.service';
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
    MaterialModule,
    BrowserModule,
    HttpClientModule,
    BrowserAnimationsModule,
    EffectsModule.forRoot([FunctionEffects, StateMachineEffects, StateMachineInstancesEffects]),
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
  providers: [FunctionService, StateMachinesService, StateMachineInstancesService],
  bootstrap: [AppComponent]
})
export class AppModule { }