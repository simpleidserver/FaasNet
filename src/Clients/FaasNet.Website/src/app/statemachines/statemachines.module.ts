import { NgModule } from "@angular/core";
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { AsyncApiEditorModule } from "../../components/asyncapi-editor/asyncapieditormodule";
import { LoaderModule } from "../../components/loader/loader.module";
import { MessagesModule } from "../../components/messages/messagesmodule";
import { MonacoEditorModule } from "../../components/monaco-editor/editor.module";
import { StateDiagramModule } from "../../components/statediagram/statediagram.module";
import { AsyncApiEditorComponent } from "./edit/asyncapi/asyncapieditor.component";
import { EditStateMachineComponent } from "./edit/edit.component";
import { EditEventDialogComponent } from "./edit/events/editevent-dialog.component";
import { StateMachineEventsEditorComponent } from "./edit/events/eventseditor.component";
import { EditFunctionDialogComponent } from "./edit/functionseditor/editfunction-dialog.component";
import { FunctionsEditorComponent } from "./edit/functionseditor/functionseditor.component";
import { StateMachineInfoComponent } from "./edit/info/info.component";
import { JsonComponent } from "./edit/json/json.component";
import { LaunchStateMachineComponent } from "./edit/launch/launch-statemachine.component";
import { StateMachineMessagesEditorComponent } from "./edit/messages/messageseditor.component";
import { YamlComponent } from "./edit/yaml/yaml.component";
import { AddStateMachineComponent } from "./list/add-statemachine.component";
import { ListStateMachinesComponent } from "./list/list.component";
import { StateMachinesRoutes } from "./statemachines.routes";

@NgModule({
  imports: [
    MaterialModule,
    SharedModule,
    StateMachinesRoutes,
    StateDiagramModule,
    MonacoEditorModule.forRoot(),
    LoaderModule,
    AsyncApiEditorModule,
    MessagesModule
  ],
  declarations: [
    EditStateMachineComponent,
    ListStateMachinesComponent,
    AddStateMachineComponent,
    LaunchStateMachineComponent,
    JsonComponent,
    YamlComponent,
    FunctionsEditorComponent,
    EditFunctionDialogComponent,
    AsyncApiEditorComponent,
    StateMachineMessagesEditorComponent,
    StateMachineInfoComponent,
    StateMachineEventsEditorComponent,
    EditEventDialogComponent
  ],
  entryComponents: [
    AddStateMachineComponent,
    LaunchStateMachineComponent,
    EditFunctionDialogComponent,
    EditEventDialogComponent
  ]
})

export class StateMachinesModule { }
