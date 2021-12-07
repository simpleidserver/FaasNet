import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'json',
  templateUrl: './json.component.html',
  styleUrls: [
    './json.component.scss',
    '../state-editor.component.scss'
  ]
})
export class JsonComponent {
  json: string = "";
  jsonOptions = { theme: 'vs', language: 'json' };

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (data && data.stateMachine) {
      this.json = JSON.stringify(data.stateMachine.getJson());
    }
  }
}
