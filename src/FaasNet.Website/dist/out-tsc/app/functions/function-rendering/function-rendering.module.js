import { __decorate } from "tslib";
import { NgModule } from "@angular/core";
import { MaterialModule } from "../../shared/material.module";
import { SharedModule } from "../../shared/shared.module";
import { ArrayRenderingComponent } from "./array/array-rendering.component";
import { FunctionRenderingComponent } from "./function-rendering.component";
import { StringRenderingComponent } from "./string/string-rendering.component";
let FunctionRenderingModule = class FunctionRenderingModule {
};
FunctionRenderingModule = __decorate([
    NgModule({
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
], FunctionRenderingModule);
export { FunctionRenderingModule };
//# sourceMappingURL=function-rendering.module.js.map