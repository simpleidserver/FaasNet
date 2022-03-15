import { NgModule } from "@angular/core";
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { AsyncApiEditorModule } from "../../components/asyncapi-editor/asyncapieditormodule";
import { LoaderModule } from "../../components/loader/loader.module";
import { DomainsRoutes } from "./domains.routes";
import { EditDomainComponent } from "./edit/edit.component";
import { EditorDomainComponent } from "./edit/editor/editor.component";
import { MessagesDomainComponent } from "./edit/messages/messages.component";
import { AddApplicationDomainComponent } from "./list/add-applicationdomain.component";
import { ListApplicationDomainComponent } from "./list/list.component";

@NgModule({
  imports: [
    MaterialModule,
    SharedModule,
    DomainsRoutes,
    LoaderModule,
    AsyncApiEditorModule
  ],
  declarations: [
    EditDomainComponent,
    ListApplicationDomainComponent,
    AddApplicationDomainComponent,
    EditorDomainComponent,
    MessagesDomainComponent
  ],
  entryComponents: [
    AddApplicationDomainComponent
  ]
})

export class DomainsModule { }
