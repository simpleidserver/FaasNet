import { NgModule } from "@angular/core";
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { AsyncApiEditorModule } from "../../components/asyncapi-editor/asyncapieditormodule";
import { LoaderModule } from "../../components/loader/loader.module";
import { DomainsRoutes } from "./domains.routes";
import { EditDomainComponent } from "./edit/edit.component";

@NgModule({
  imports: [
    MaterialModule,
    SharedModule,
    DomainsRoutes,
    LoaderModule,
    AsyncApiEditorModule
  ],
  declarations: [
    EditDomainComponent
  ],
  entryComponents: [
  ]
})

export class DomainsModule { }
