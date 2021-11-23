import { NgModule } from "@angular/core";
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
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
    StateDiagramComponent
  ],
  entryComponents: [
  ]
})

export class StateMachinesModule { }
