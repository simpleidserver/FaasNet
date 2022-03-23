import { NgModule } from '@angular/core';
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { MonacoEditorModule } from '../monaco-editor/editor.module';
import { AsyncApiEditorComponent } from './asyncapi-editor.component';
import { ApplicationEditorComponent } from './components/application/application-editor.component';
import { ChooseClientComponent } from './components/application/chooseclient.component';
import { ChooseMessageComponent } from './components/link/choosemessage.component';
import { LinkEditorComponent } from './components/link/link-editor.component';
import { ViewAsyncApiComponent } from './viewasyncapicomponent';

@NgModule({
  imports: [
    MonacoEditorModule.forRoot(),
    SharedModule,
    MaterialModule,
  ],
  declarations: [
    AsyncApiEditorComponent,
    ApplicationEditorComponent,
    LinkEditorComponent,
    ChooseMessageComponent,
    ViewAsyncApiComponent,
    ChooseClientComponent
  ],
  exports: [
    AsyncApiEditorComponent
  ]
})

export class AsyncApiEditorModule { }
