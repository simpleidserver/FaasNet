import { NgModule } from "@angular/core";
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { AsyncApiEditorModule } from "../../components/asyncapi-editor/asyncapieditormodule";
import { LoaderModule } from "../../components/loader/loader.module";
import { AddVpnComponent } from "./list/add-vpn.component";
import { ListVpnComponent } from "./list/list.component";
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
    AddVpnComponent
  ],
  entryComponents: [
    AddVpnComponent
  ]
})

export class VpnsModule { }
