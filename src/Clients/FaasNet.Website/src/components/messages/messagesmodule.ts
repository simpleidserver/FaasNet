import { NgModule } from '@angular/core';
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { MonacoEditorModule } from '../monaco-editor/editor.module';
import { AddMessageDefComponent } from './add-message.component';
import { MessagesVpnComponent } from './messages.component';

@NgModule({
  imports: [
    MonacoEditorModule.forRoot(),
    SharedModule,
    MaterialModule
  ],
  declarations: [
    MessagesVpnComponent,
    AddMessageDefComponent
  ],
  exports: [
    MessagesVpnComponent,
    AddMessageDefComponent
  ]
})

export class MessagesModule { }
