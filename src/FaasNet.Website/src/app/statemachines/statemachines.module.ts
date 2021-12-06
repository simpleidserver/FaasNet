import { NgModule } from "@angular/core";
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { StateDiagramModule } from "../../components/statediagram/statediagram.module";
import { EditStateMachineComponent } from "./edit/edit.component";
import { StateMachinesRoutes } from "./statemachines.routes";

@NgModule({
  imports: [
    MaterialModule,
    SharedModule,
    StateMachinesRoutes,
    StateDiagramModule
  ],
  declarations: [
    EditStateMachineComponent
  ],
  entryComponents: [
  ]
})

export class StateMachinesModule { }
