var MonacoEditorModule_1;
import { __decorate } from "tslib";
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { NGX_MONACO_EDITOR_CONFIG } from './config';
import { EditorComponent } from './monaco-editor.component';
let MonacoEditorModule = MonacoEditorModule_1 = class MonacoEditorModule {
    static forRoot(config = {}) {
        return {
            ngModule: MonacoEditorModule_1,
            providers: [
                { provide: NGX_MONACO_EDITOR_CONFIG, useValue: config }
            ]
        };
    }
};
MonacoEditorModule = MonacoEditorModule_1 = __decorate([
    NgModule({
        imports: [
            CommonModule
        ],
        declarations: [
            EditorComponent
        ],
        exports: [
            EditorComponent
        ]
    })
], MonacoEditorModule);
export { MonacoEditorModule };
//# sourceMappingURL=editor.module.js.map