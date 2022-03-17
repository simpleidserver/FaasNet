import { __decorate } from "tslib";
import { NgModule } from '@angular/core';
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { MonacoEditorModule } from '../monaco-editor/editor.module';
import { AsyncApiEditorComponent } from './asyncapi-editor.component';
import { ApplicationEditorComponent } from './components/application/application-editor.component';
import { EditLinkDialogComponent } from './components/link/editlink-dialog.component';
import { LinkEventsEditorComponent } from './components/link/evtseditor.component';
import { LinkEditorComponent } from './components/link/link-editor.component';
import { ViewAsyncApiComponent } from './viewasyncapicomponent';
let AsyncApiEditorModule = class AsyncApiEditorModule {
};
AsyncApiEditorModule = __decorate([
    NgModule({
        imports: [
            MonacoEditorModule.forRoot(),
            SharedModule,
            MaterialModule,
        ],
        declarations: [
            AsyncApiEditorComponent,
            ApplicationEditorComponent,
            LinkEditorComponent,
            LinkEventsEditorComponent,
            EditLinkDialogComponent,
            ViewAsyncApiComponent
        ],
        exports: [
            AsyncApiEditorComponent
        ]
    })
], AsyncApiEditorModule);
export { AsyncApiEditorModule };
//# sourceMappingURL=asyncapieditormodule.js.map