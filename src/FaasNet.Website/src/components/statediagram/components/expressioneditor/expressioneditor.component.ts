import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
declare var jq: any;

@Component({
  selector: 'expressioneditor',
  templateUrl: './expressioneditor.component.html',
  styleUrls: [
    './expressioneditor.component.scss',
    '../state-editor.component.scss'
  ]
})
export class ExpressionEditorComponent {
  private _filter: string = "";
  private _json: string = "{}";
  private _result: string = "{}";
  private _modelFilterOptions: any = null;
  jsonEditorOptions = {
    theme: 'vs',
    language: 'json',
    minimap: { enabled: false },
    overviewRulerBorder: false,
    overviewRulerLanes: 0,
    lineNumbers: 'off',
    lineNumbersMinChars: 0,
    lineDecorationsWidth: 0,
    renderLineHighlight: 'none',
    scrollbar: {
      horizontal: 'hidden',
      vertical: 'hidden',
      alwaysConsumeMouseWheel: false,
    }
  };
  filterOptions = { theme: 'vs', language: 'jq', isSingleLine: true };
  get filter() {
    return this._filter;
  }
  set filter(f: string) {
    this._filter = f;
    this.applyFilter();
  }
  get json() {
    return this._json;
  }
  set json(j: string) {
    this._json = j;
    this.updateFilter();
    this.applyFilter();
  }
  get result() {
    return this._result;
  }
  set result(r : string) {
    this._result = r;
  }

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<ExpressionEditorComponent>) {
    if (data && data.filter) {
      var filter = data.filter.replace('$', '')
        .replace('{', '')
        .replace('}', '')
        .trim();
      this._filter = filter;
    }
  }

  save() {
    if (!this._filter || this.filter === "") {
      this.dialogRef.close({
        filter: null
      });
      return;
    }

    let filter = "${ " + this._filter + " }";
    this.dialogRef.close({
      filter: filter
    });
  }

  ngAfterViewInit() {
  }

  initFilterOptions(evt: any) {
    this._modelFilterOptions = evt.getModel();
  }

  private updateFilter() {
    if (!this._modelFilterOptions) {
      return;
    }

    let json: any = null;
    try {
      json = JSON.parse(this._json);
    } catch { }
    Object.assign(this._modelFilterOptions, {
      json: json
    });
  }

  private applyFilter() {
    let jObj: any = null;
    let result: string = "";
    try {
      jObj = JSON.parse(this._json);
    }
    catch { }

    if (jObj && this._filter) {
      try {
        const record = jq.json(jObj, this._filter);
        if (typeof record == "object") {
          result = JSON.stringify(record);
        } else {
          result = record.toString();
        }
      }
      catch { }
    }

    this.result = result;
  }
}
