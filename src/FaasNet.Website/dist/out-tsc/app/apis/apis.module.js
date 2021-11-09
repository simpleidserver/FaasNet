import { __decorate } from "tslib";
import { NgModule } from "@angular/core";
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { ChartsModule } from "ng2-charts";
import { FunctionRenderingModule } from "../functions/function-rendering/function-rendering.module";
import { ApisRoutes } from "./apis.routes";
import { EditApiComponent } from "./edit/edit.component";
import { AddFunctionComponent } from "./edit/function-panel/add-function.component";
import { FunctionPanelComponent } from "./edit/function-panel/function-panel.component";
import { UpdateFunctionConfigurationComponent } from "./edit/function-panel/update-configuration.component";
import { LaunchFunctionDialogComponent } from "./edit/launch-function-dialog.component";
import { AddApiDefComponent } from "./list/add-api.component";
import { ListApiDefComponent } from "./list/list.component";
import { InfoApiComponent } from "./view/info/info.component";
import { AddOperationComponent } from "./view/operations/add-operation.component";
import { OperationsApiComponents } from "./view/operations/operations.component";
import { ViewApiDefComponent } from "./view/view.component";
let ApisModule = class ApisModule {
};
ApisModule = __decorate([
    NgModule({
        imports: [
            MaterialModule,
            SharedModule,
            ApisRoutes,
            ChartsModule,
            FunctionRenderingModule
        ],
        declarations: [
            EditApiComponent,
            AddFunctionComponent,
            UpdateFunctionConfigurationComponent,
            FunctionPanelComponent,
            AddApiDefComponent,
            ListApiDefComponent,
            ViewApiDefComponent,
            InfoApiComponent,
            OperationsApiComponents,
            AddOperationComponent,
            LaunchFunctionDialogComponent
        ],
        entryComponents: [
            AddApiDefComponent,
            AddOperationComponent,
            LaunchFunctionDialogComponent
        ]
    })
], ApisModule);
export { ApisModule };
//# sourceMappingURL=apis.module.js.map