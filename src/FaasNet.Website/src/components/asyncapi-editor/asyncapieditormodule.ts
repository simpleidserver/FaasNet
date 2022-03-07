import { NgModule } from '@angular/core';
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { MonacoEditorModule } from '../monaco-editor/editor.module';
import { AsyncApiEditorComponent } from './asyncapi-editor.component';

@NgModule({
  imports: [
    MonacoEditorModule.forRoot(),
    SharedModule,
    MaterialModule,
  ],
  declarations: [
    AsyncApiEditorComponent
  ],
  exports: [
    AsyncApiEditorComponent
  ]
})

export class AsyncApiEditorModule { }
