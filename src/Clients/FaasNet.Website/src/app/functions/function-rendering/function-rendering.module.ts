import { NgModule } from "@angular/core";
import { MaterialModule } from "../../shared/material.module";
import { SharedModule } from "../../shared/shared.module";
import { ArrayRenderingComponent } from "./array/array-rendering.component";
import { FunctionRenderingComponent } from "./function-rendering.component";
import { StringRenderingComponent } from "./string/string-rendering.component";

@NgModule({
  imports: [
    MaterialModule,
    SharedModule
  ],
  exports: [
    FunctionRenderingComponent,
    ArrayRenderingComponent,
    StringRenderingComponent
  ],
  declarations: [
    FunctionRenderingComponent,
    ArrayRenderingComponent,
    StringRenderingComponent
  ]
})

export class FunctionRenderingModule { }
