import { __decorate } from "tslib";
import { NgModule } from "@angular/core";
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { LoaderModule } from "../../components/loader/loader.module";
import { MonacoEditorModule } from "../../components/monaco-editor/editor.module";
import { StateDiagramModule } from "../../components/statediagram/statediagram.module";
import { ListStateMachineInstanceComponent } from "./list/list.component";
import { StateMachineInstancesRoutes } from "./statemachineinstances.routes";
import { ViewStateMachineInstanceComponent } from "./view/view.component";
let StateMachineInstancesModule = class StateMachineInstancesModule {
};
StateMachineInstancesModule = __decorate([
    NgModule({
        imports: [
            MaterialModule,
            SharedModule,
            StateMachineInstancesRoutes,
            StateDiagramModule,
            MonacoEditorModule.forRoot(),
            LoaderModule
        ],
        declarations: [
            ListStateMachineInstanceComponent,
            ViewStateMachineInstanceComponent
        ],
        entryComponents: []
    })
], StateMachineInstancesModule);
export { StateMachineInstancesModule };
//# sourceMappingURL=statemachineinstances.module.js.map