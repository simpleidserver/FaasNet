import { NgModule } from "@angular/core";
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { LoaderModule } from "../../components/loader/loader.module";
import { EventMeshServersRoutes } from "./eventmeshservers.routes";
import { AddEventMeshServerComponent } from "./list/add-eventmeshserver.component";
import { AddBridgeComponent } from "./list/addbridge.component";
import { ListEventMeshServersComponent } from "./list/list.component";

@NgModule({
  imports: [
    MaterialModule,
    SharedModule,
    EventMeshServersRoutes,
    LoaderModule
  ],
  declarations: [
    ListEventMeshServersComponent,
    AddEventMeshServerComponent,
    AddBridgeComponent
  ],
  entryComponents: [
    AddEventMeshServerComponent
  ]
})

export class EventMeshServersModule { }
