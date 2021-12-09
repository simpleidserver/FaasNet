import { NgModule } from "@angular/core";
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { LoaderComponent } from "../../components/loader/loader.component";
import { MonacoEditorModule } from "../../components/monaco-editor/editor.module";
import { StateDiagramModule } from "../../components/statediagram/statediagram.module";
import { EditStateMachineComponent } from "./edit/edit.component";
import { LaunchStateMachineComponent } from "./launch/launch-statemachine.component";
import { AddStateMachineComponent } from "./list/add-statemachine.component";
import { ListStateMachinesComponent } from "./list/list.component";
import { StateMachinesRoutes } from "./statemachines.routes";

@NgModule({
  imports: [
    MaterialModule,
    SharedModule,
    StateMachinesRoutes,
    StateDiagramModule,
    MonacoEditorModule.forRoot()
  ],
  declarations: [
    EditStateMachineComponent,
    ListStateMachinesComponent,
    AddStateMachineComponent,
    LoaderComponent,
    LaunchStateMachineComponent
  ],
  entryComponents: [
    AddStateMachineComponent,
    LaunchStateMachineComponent
  ]
})

export class StateMachinesModule { }
