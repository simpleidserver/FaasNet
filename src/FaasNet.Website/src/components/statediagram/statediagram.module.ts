import { NgModule } from '@angular/core';
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { MonacoEditorModule } from '../monaco-editor/editor.module';
import { DataConditionComponent } from './components/datacondition/datacondition.component';
import { EvtConditionComponent } from './components/evtcondition/evtcondition.component';
import { ExpressionEditorComponent } from './components/expressioneditor/expressioneditor.component';
import { InjectStateEditorComponent } from './components/inject/inject-state-editor.component';
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
    SwitchStateEditorComponent
  ],
  exports: [
    StateDiagramComponent,
    DataConditionComponent,
    EvtConditionComponent,
    ExpressionEditorComponent,
    InjectStateEditorComponent,
    SwitchStateEditorComponent
  ]
})

export class StateDiagramModule { }
