import { NgModule } from "@angular/core";
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { EventMeshServersRoutes } from "./eventmeshservers.routes";
import { AddEventMeshServerComponent } from "./list/add-eventmeshserver.component";
import { ListEventMeshServersComponent } from "./list/list.component";

@NgModule({
  imports: [
    MaterialModule,
    SharedModule,
    EventMeshServersRoutes
  ],
  declarations: [
    ListEventMeshServersComponent,
    AddEventMeshServerComponent
  ],
  entryComponents: [
    AddEventMeshServerComponent
  ]
})

export class EventMeshServersModule { }
