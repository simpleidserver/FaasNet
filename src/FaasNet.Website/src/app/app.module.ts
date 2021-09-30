import { HttpClient, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { environment } from '@envs/environment';
import { EffectsModule } from '@ngrx/effects';
import { StoreModule } from '@ngrx/store';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { appReducer } from '../stores/appstate';
import { FunctionEffects } from '../stores/functions/effects/function.effects';
import { FunctionService } from '../stores/functions/services/function.service';
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
    EffectsModule.forRoot([FunctionEffects]),
    StoreModule.forRoot(appReducer),
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateLoader,
        deps: [HttpClient]
      }
    })
  ],
  providers: [FunctionService],
  bootstrap: [AppComponent]
})
export class AppModule { }
