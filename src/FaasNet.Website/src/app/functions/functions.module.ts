import { NgModule } from "@angular/core";
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { ArrayRenderingComponent } from "./function-rendering/array/array-rendering.component";
import { FunctionRenderingComponent } from "./function-rendering/function-rendering.component";
import { StringRenderingComponent } from "./function-rendering/string/string-rendering.component";
import { FunctionsRoutes } from "./functions.routes";
import { ListFunctionsComponent } from "./list/list.component";
import { ViewFunctionComponent } from "./view/view.component";

@NgModule({
  imports: [
    MaterialModule,
    SharedModule,
    FunctionsRoutes
  ],
  declarations: [
    ListFunctionsComponent,
    ViewFunctionComponent,
    FunctionRenderingComponent,
    ArrayRenderingComponent,
    StringRenderingComponent
  ]
})

export class FunctionsModule { }
