import { Component, EventEmitter, Input, Output } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { BaseTransition } from "@stores/statemachines/models/statemachine-state.model";
import { DataCondition } from "@stores/statemachines/models/statemachine-switch-state.model";
import { ExpressionEditorComponent } from "../expressioneditor/expressioneditor.component";

@Component({
  selector: 'datacondition-editor',
  templateUrl: './datacondition.component.html',
  styleUrls: ['../state-editor.component.scss']
})
export class DataConditionComponent {
  private conditionRefSubscription: any | null = null;
  private _dataCondition: DataCondition | null = null;
  private _transition: BaseTransition | null = null;
  @Output() closed: EventEmitter<any> = new EventEmitter<any>();
  @Input()
  get transition(): BaseTransition | null {
    return this._transition;
  }
  set transition(v: BaseTransition | null) {
    this._transition = v;
    this._dataCondition = v as DataCondition;
    this.init();
  }

  updateDataConditionFormGroup: FormGroup = new FormGroup({
    condition: new FormControl()
  });

  constructor(private dialog: MatDialog) {
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    if (this.conditionRefSubscription) {
      this.conditionRefSubscription.unsubscribe();
    }
  }

  close() {
    this.closed.emit();
  }

  editExpression() {
    let filter: string = "";
    const conditionFormControl = this.updateDataConditionFormGroup.get('condition');
    if (conditionFormControl) {
      filter = conditionFormControl.value;
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

      if (this._dataCondition) {
        this._dataCondition.condition = r.filter;
        this.updateDataConditionFormGroup.get('condition')?.setValue(this._dataCondition?.condition);
      }
    });
  }

  private init() {
    const self = this;
    this.ngOnDestroy();
    this.updateDataConditionFormGroup.get('condition')?.setValue(this._dataCondition?.condition);
    this.conditionRefSubscription = this.updateDataConditionFormGroup.get('condition')?.valueChanges.subscribe((e: any) => {
      if (self._dataCondition) {
        self._dataCondition.condition = e;
      }
    });
  }
}
