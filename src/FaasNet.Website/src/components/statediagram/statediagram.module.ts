import { NgModule } from '@angular/core';
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { MonacoEditorModule } from '../monaco-editor/editor.module';
import { DataConditionComponent } from './components/datacondition/datacondition.component';
import { DefaultConditionComponent } from './components/defaultcondition/defaultcondition.component';
import { EvtConditionComponent } from './components/evtcondition/evtcondition.component';
import { ExpressionEditorComponent } from './components/expressioneditor/expressioneditor.component';
import { InjectStateEditorComponent } from './components/inject/inject-state-editor.component';
import { JsonComponent } from './components/json/json.component';
import { SwitchStateEditorComponent } from './components/switch/switch-state-editor.component';
import { YamlComponent } from './components/yaml/yaml.component';
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
    YamlComponent,
    JsonComponent
  ],
  exports: [
    StateDiagramComponent,
    DataConditionComponent,
    EvtConditionComponent,
    ExpressionEditorComponent,
    InjectStateEditorComponent,
    SwitchStateEditorComponent,
    DefaultConditionComponent,
    YamlComponent,
    JsonComponent
  ]
})

export class StateDiagramModule { }
