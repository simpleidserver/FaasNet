import { NgModule } from "@angular/core";
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { MonacoEditorModule } from "../../components/monaco-editor/editor.module";
import { StateDiagramModule } from "../../components/statediagram/statediagram.module";
import { ListStateMachineInstanceComponent } from "./list/list.component";
import { StateMachineInstancesRoutes } from "./statemachineinstances.routes";

@NgModule({
  imports: [
    MaterialModule,
    SharedModule,
    StateMachineInstancesRoutes,
    StateDiagramModule,
    MonacoEditorModule.forRoot()
  ],
  declarations: [
    ListStateMachineInstanceComponent
  ],
  entryComponents: [
  ]
})

export class StateMachineInstancesModule { }
