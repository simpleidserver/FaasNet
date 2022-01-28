import { NgModule } from '@angular/core';
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { MatPanelComponent } from '../matpanel/matpanel.component';
import { MonacoEditorModule } from '../monaco-editor/editor.module';
import { DataConditionComponent } from './components/datacondition/datacondition.component';
import { DefaultConditionComponent } from './components/defaultcondition/defaultcondition.component';
import { EvtConditionComponent } from './components/evtcondition/evtcondition.component';
import { ExpressionEditorComponent } from './components/expressioneditor/expressioneditor.component';
import { InjectStateEditorComponent } from './components/inject/inject-state-editor.component';
import { ActionsEditorComponent } from './components/operation/actionseditor.component';
import { OperationStateEditorComponent } from './components/operation/operation-state-editor.component';
import { SwitchStateEditorComponent } from './components/switch/switch-state-editor.component';
import { StateDiagramComponent } from './statediagram.component';

@NgModule({
  imports: [
    MonacoEditorModule.forRoot(),
    SharedModule,
    MaterialModule,
  ],
  declarations: [
    StateDiagramComponent,
    DataConditionComponent,
    EvtConditionComponent,
    ExpressionEditorComponent,
    InjectStateEditorComponent,
    SwitchStateEditorComponent,
    DefaultConditionComponent,
    OperationStateEditorComponent,
    ActionsEditorComponent,
    MatPanelComponent
  ],
  exports: [
    StateDiagramComponent,
    DataConditionComponent,
    EvtConditionComponent,
    ExpressionEditorComponent,
    InjectStateEditorComponent,
    SwitchStateEditorComponent,
    DefaultConditionComponent,
    OperationStateEditorComponent,
    ActionsEditorComponent,
    MatPanelComponent
  ]
})

export class StateDiagramModule { }
