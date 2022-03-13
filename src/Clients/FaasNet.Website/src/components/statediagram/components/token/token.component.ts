import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'display-token-json',
  templateUrl: './token.component.html',
  styleUrls: [
    './token.component.scss',
  ]
})
export class TokenComponent {
  json: string = "";
  jsonOptions = { theme: 'vs', language: 'json', automaticLayout: true };

  constructor(@Inject(MAT_DIALOG_DATA) public data: any) {
    this.json = JSON.stringify(data, null, "\t");
  }

}
