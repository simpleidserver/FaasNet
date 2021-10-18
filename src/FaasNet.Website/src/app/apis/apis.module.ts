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

@NgModule({
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
    FunctionPanelComponent
  ]
})

export class ApisModule { }
