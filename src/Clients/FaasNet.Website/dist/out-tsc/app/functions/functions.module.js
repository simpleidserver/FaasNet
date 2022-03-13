import { __decorate } from "tslib";
import { NgModule } from "@angular/core";
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { ChartsModule } from "ng2-charts";
import { FunctionRenderingModule } from "./function-rendering/function-rendering.module";
import { FunctionsRoutes } from "./functions.routes";
import { AddFunctionComponent } from "./list/add-function.component";
import { ListFunctionsComponent } from "./list/list.component";
import { InfoFunctionComponent } from "./view/info/info.component";
import { InvokeFunctionComponent } from "./view/invoke/invoke.component";
import { MonitoringFunctionComponent } from "./view/monitoring/monitoring.component";
import { ViewFunctionComponent } from "./view/view.component";
let FunctionsModule = class FunctionsModule {
};
FunctionsModule = __decorate([
    NgModule({
        imports: [
            MaterialModule,
            SharedModule,
            FunctionsRoutes,
            ChartsModule,
            FunctionRenderingModule
        ],
        declarations: [
            ListFunctionsComponent,
            ViewFunctionComponent,
            InvokeFunctionComponent,
            InfoFunctionComponent,
            AddFunctionComponent,
            MonitoringFunctionComponent
        ]
    })
], FunctionsModule);
export { FunctionsModule };
//# sourceMappingURL=functions.module.js.map