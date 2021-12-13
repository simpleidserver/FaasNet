import { Component, EventEmitter, Input, Output } from "@angular/core";
import { FormControl } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { InjectStateMachineState } from "@stores/statemachines/models/statemachine-inject-state.model";
import { StateMachineState } from "@stores/statemachines/models/statemachine-state.model";
import { ExpressionEditorComponent } from "../expressioneditor/expressioneditor.component";

@Component({
  selector: 'inject-state-editor',
  templateUrl: './inject-state-editor.component.html',
  styleUrls: ['../state-editor.component.scss']
})
export class InjectStateEditorComponent {
  private _state: StateMachineState | null = null;
  private _injectState: InjectStateMachineState | null = null;
  private _data: string = "";
  private nameSubscription: any | null = null;
  private dataSubscription: any | null = null;
  @Output() closed: EventEmitter<any> = new EventEmitter<any>();
  @Input()
  get state(): StateMachineState | null {
    return this._state;
  }
  set state(v : StateMachineState | null) {
    this._state = v;
    this._injectState = v as InjectStateMachineState;
    this.init();
  }

  get data(): string {
    return this._data;
  }

  set data(str: string) {
    this._data = str;
    if (this._injectState) {
      try {
        const data = JSON.parse(str);
        this._injectState.data = data;
      }
      catch { }
    }
  }

  get end() {
    return this._injectState?.end;
  }

  inputStateDataFilter: string = "";
  outputStateDataFilter: string = "";

  jsonOptions: any = {
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

  nameFormControl: FormControl = new FormControl();

  constructor(private dialog : MatDialog) {
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    if (this.nameSubscription) {
      this.nameSubscription.unsubscribe();
    }

    if (this.dataSubscription) {
      this.dataSubscription.unsubscribe();
    }
  }

  close() {
    this.closed.emit();
  }

  editInputStateDataFilter() {
    let filter: string = "";
    if (this._injectState?.stateDataFilter?.input) {
      filter = this._injectState?.stateDataFilter.input;
    }

    const dialogRef = this.dialog.open(ExpressionEditorComponent, {
      width: '800px',
      data: {
        filter: filter
      }
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r || !r.filter) {
        return;
      }

      if (this._injectState?.stateDataFilter) {
        this._injectState.stateDataFilter.input = r.filter;
      }

      this.inputStateDataFilter = r.filter;
    });
  }

  editOutputStateDataFilter() {
    let filter: string = "";
    if (this._injectState?.stateDataFilter?.output) {
      filter = this._injectState?.stateDataFilter.output;
    }

    const dialogRef = this.dialog.open(ExpressionEditorComponent, {
      width: '800px',
      data: {
        filter: filter
      }
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r || !r.filter) {
        return;
      }

      if (this._injectState?.stateDataFilter) {
        this._injectState.stateDataFilter.output = r.filter;
      }

      this.outputStateDataFilter = r.filter;
    });
  }

  private init() {
    const self = this;
    this.ngOnDestroy();
    let json: any = "{}";
    if (this._injectState && this._injectState.data) {
      json = JSON.stringify(this._injectState.data);
    }

    this._data = json;
    this.nameFormControl.setValue(this._injectState?.name);
    this.nameSubscription = this.nameFormControl.valueChanges.subscribe((e: any) => {
      if (self._injectState) {
        self._injectState.name = e;
      }
    });
  }
}
