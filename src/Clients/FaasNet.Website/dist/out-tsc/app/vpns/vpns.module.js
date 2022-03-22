import { __decorate } from "tslib";
import { NgModule } from "@angular/core";
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { AsyncApiEditorModule } from "../../components/asyncapi-editor/asyncapieditormodule";
import { LoaderModule } from "../../components/loader/loader.module";
import { AddVpnComponent } from "./list/add-vpn.component";
import { ListVpnComponent } from "./list/list.component";
import { AddAppDomainComponent } from "./view/appdomains/add-appdomain.component";
import { AppDomainsVpnComponent } from "./view/appdomains/appdomains.component";
import { MessagesVpnComponent } from "./view/appdomains/view/messages/messages.component";
import { ViewVpnAppDomainComponent } from "./view/appdomains/view/view.component";
import { AddClientComponent } from "./view/clients/add-client.component";
import { ClientsVpnComponent } from "./view/clients/clients.component";
import { ViewVpnClientSessionsComponent } from "./view/clients/view/sessions/sessions.component";
import { ViewVpnClientComponent } from "./view/clients/view/view.component";
import { InfoVpnComponent } from "./view/info/info.component";
import { ViewVpnComponent } from "./view/view.component";
import { VpnsRoutes } from "./vpns.routes";
let VpnsModule = class VpnsModule {
};
VpnsModule = __decorate([
    NgModule({
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
            ClientsVpnComponent,
            AddClientComponent,
            ViewVpnClientComponent,
            ViewVpnClientSessionsComponent,
            AppDomainsVpnComponent,
            AddAppDomainComponent,
            ViewVpnAppDomainComponent,
            MessagesVpnComponent
        ],
        entryComponents: [
            AddVpnComponent,
            AddClientComponent,
            AddAppDomainComponent
        ]
    })
], VpnsModule);
export { VpnsModule };
//# sourceMappingURL=vpns.module.js.map