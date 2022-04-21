import { NgModule } from "@angular/core";
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { AsyncApiEditorModule } from "../../components/asyncapi-editor/asyncapieditormodule";
import { LoaderModule } from "../../components/loader/loader.module";
import { MessagesModule } from "../../components/messages/messagesmodule";
import { MonacoEditorModule } from "../../components/monaco-editor/editor.module";
import { SafePipe } from "../pipes/safe.pipe";
import { EventMeshLoggingComponent } from "./eventmeshlogging.component";
import { EventMeshLoggingRoutes } from "./eventmeshlogging.routes";

@NgModule({
  imports: [
    MaterialModule,
    MonacoEditorModule.forRoot(),
    SharedModule,
    EventMeshLoggingRoutes,
    LoaderModule,
    AsyncApiEditorModule,
    MessagesModule
  ],
  declarations: [
    EventMeshLoggingComponent,
    SafePipe
  ]
})

export class EventMeshLoggingModule { }
