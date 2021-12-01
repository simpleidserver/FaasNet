import { NgModule } from "@angular/core";
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { InjectStateEditorComponent } from "../../components/statediagram/components/inject/inject-state-editor.component";
import { SwitchStateEditorComponent } from "../../components/statediagram/components/switch/switch-state-editor.component";
import { StateDiagramComponent } from "../../components/statediagram/statediagram.component";
import { EditStateMachineComponent } from "./edit/edit.component";
import { StateMachinesRoutes } from "./statemachines.routes";

@NgModule({
  imports: [
    MaterialModule,
    SharedModule,
    StateMachinesRoutes
  ],
  declarations: [
    EditStateMachineComponent,
    StateDiagramComponent,
    SwitchStateEditorComponent,
    InjectStateEditorComponent
  ],
  entryComponents: [
  ]
})

export class StateMachinesModule { }
