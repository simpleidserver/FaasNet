import { NgModule } from "@angular/core";
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { AsyncApiEditorModule } from "../../components/asyncapi-editor/asyncapieditormodule";
import { LoaderModule } from "../../components/loader/loader.module";
import { AddVpnComponent } from "./list/add-vpn.component";
import { ListVpnComponent } from "./list/list.component";
import { ClientsVpnComponent } from "./view/clients/clients.component";
import { InfoVpnComponent } from "./view/info/info.component";
import { ViewVpnComponent } from "./view/view.component";
import { VpnsRoutes } from "./vpns.routes";

@NgModule({
  imports: [
    MaterialModule,
    SharedModule,
    VpnsRoutes,
    LoaderModule,
    AsyncApiEditorModule
  ],
  declarations: [
    ListVpnComponent,
    AddVpnComponent,
    InfoVpnComponent,
    ViewVpnComponent,
    ClientsVpnComponent
  ],
  entryComponents: [
    AddVpnComponent
  ]
})

export class VpnsModule { }
