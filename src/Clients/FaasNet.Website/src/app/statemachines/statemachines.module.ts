import { NgModule } from "@angular/core";
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { AsyncApiEditorModule } from "../../components/asyncapi-editor/asyncapieditormodule";
import { LoaderModule } from "../../components/loader/loader.module";
import { MonacoEditorModule } from "../../components/monaco-editor/editor.module";
import { StateDiagramModule } from "../../components/statediagram/statediagram.module";
import { AsyncApiEditorComponent } from "./edit/asyncapi/asyncapieditor.component";
import { EditStateMachineComponent } from "./edit/edit.component";
import { EditFunctionDialogComponent } from "./edit/functionseditor/editfunction-dialog.component";
import { FunctionsEditorComponent } from "./edit/functionseditor/functionseditor.component";
import { JsonComponent } from "./edit/json/json.component";
import { LaunchStateMachineComponent } from "./edit/launch/launch-statemachine.component";
import { MessagesEditorComponent } from "./edit/messages/messageseditor.component";
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
    AsyncApiEditorModule
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
    MessagesEditorComponent
  ],
  entryComponents: [
    AddStateMachineComponent,
    LaunchStateMachineComponent,
    EditFunctionDialogComponent
  ]
})

export class StateMachinesModule { }
