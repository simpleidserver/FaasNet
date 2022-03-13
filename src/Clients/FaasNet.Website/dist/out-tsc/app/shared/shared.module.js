import { __decorate } from "tslib";
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { TranslateObjPipe } from '../pipes/translateobj.pipe';
let SharedModule = class SharedModule {
};
SharedModule = __decorate([
    NgModule({
        imports: [],
        declarations: [
            TranslateObjPipe
        ],
        exports: [
            CommonModule,
            RouterModule,
            TranslateModule,
            TranslateObjPipe
        ]
    })
], SharedModule);
export { SharedModule };
//# sourceMappingURL=shared.module.js.map