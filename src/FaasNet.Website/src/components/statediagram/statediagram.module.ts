import { NgModule } from '@angular/core';
import { MaterialModule } from "@app/shared/material.module";
import { SharedModule } from "@app/shared/shared.module";
import { MonacoEditorModule } from '../monaco-editor/editor.module';
import { DataConditionComponent } from './components/datacondition/datacondition.component';
import { DefaultConditionComponent } from './components/defaultcondition/defaultcondition.component';
import { EvtConditionComponent } from './components/evtcondition/evtcondition.component';
import { ExpressionEditorComponent } from './components/expressioneditor/expressioneditor.component';
import { FunctionsEditorComponent } from './components/functionseditor/functionseditor.component';
import { InjectStateEditorComponent } from './components/inject/inject-state-editor.component';
import { JsonComponent } from './components/json/json.component';
import { ActionsEditorComponent } from './components/operation/actionseditor.component';
import { OperationStateEditorComponent } from './components/operation/operation-state-editor.component';
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
    JsonComponent,
    OperationStateEditorComponent,
    FunctionsEditorComponent,
    ActionsEditorComponent
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
    JsonComponent,
    OperationStateEditorComponent,
    FunctionsEditorComponent,
    ActionsEditorComponent
  ]
})

export class StateDiagramModule { }
