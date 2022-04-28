import { NgModule } from "@angular/core";
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { AsyncApiEditorModule } from "../../components/asyncapi-editor/asyncapieditormodule";
import { LoaderModule } from "../../components/loader/loader.module";
import { MessagesModule } from "../../components/messages/messagesmodule";
import { MonacoEditorModule } from "../../components/monaco-editor/editor.module";
import { PipeModule } from "../pipes/pipemodule";
import { StateMachineLoggingComponent } from "./statemachinelogging.component";
import { StateMachineLoggingRoutes } from "./statemachinelogging.routes";

@NgModule({
  imports: [
    MaterialModule,
    MonacoEditorModule.forRoot(),
    SharedModule,
    StateMachineLoggingRoutes,
    LoaderModule,
    AsyncApiEditorModule,
    MessagesModule,
    PipeModule
  ],
  declarations: [
    StateMachineLoggingComponent
  ]
})

export class StateMachineLoggingModule { }
