import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Document } from 'yaml';

@Component({
  selector: 'yaml',
  templateUrl: './yaml.component.html',
  styleUrls: [
    './yaml.component.scss',
    '../state-editor.component.scss'
  ]
})
export class YamlComponent {
  yaml: string = "";
  yamlOptions = { theme: 'vs', language: 'yaml' };

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<YamlComponent>) {
    if (data && data.stateMachine) {
      const doc = new Document();
      doc.contents = data.stateMachine.getJson();
      this.yaml = doc.toString();
    }
  }
}
