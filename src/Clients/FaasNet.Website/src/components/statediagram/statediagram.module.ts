import { NgModule } from '@angular/core';
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { MatPanelComponent } from '../matpanel/matpanel.component';
import { MonacoEditorModule } from '../monaco-editor/editor.module';
import { DataConditionComponent } from './components/datacondition/datacondition.component';
import { DefaultConditionComponent } from './components/defaultcondition/defaultcondition.component';
import { EvtConditionComponent } from './components/evtcondition/evtcondition.component';
import { ChooseEvtComponent } from './components/evtcondition/chooseevt.component';
import { ExpressionEditorComponent } from './components/expressioneditor/expressioneditor.component';
import { InjectStateEditorComponent } from './components/inject/inject-state-editor.component';
import { ActionsEditorComponent } from './components/operation/actionseditor.component';
import { EditActionDialogComponent } from './components/operation/editaction-dialog.component';
import { OperationStateEditorComponent } from './components/operation/operation-state-editor.component';
import { SwitchStateEditorComponent } from './components/switch/switch-state-editor.component';
import { TokenComponent } from './components/token/token.component';
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
    MatPanelComponent,
    EditActionDialogComponent,
    TokenComponent,
    ChooseEvtComponent
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
    MatPanelComponent,
    EditActionDialogComponent,
    TokenComponent
  ]
})

export class StateDiagramModule { }
