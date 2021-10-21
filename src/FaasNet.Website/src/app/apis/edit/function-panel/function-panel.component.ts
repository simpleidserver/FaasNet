import { Component, Input, OnDestroy, OnInit } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { ActivatedRoute } from "@angular/router";
import { select, Store } from "@ngrx/store";
import * as fromReducers from '@stores/appstate';
import { FunctionResult } from "@stores/functions/models/function.model";
import { startGet } from "@stores/functions/actions/function.actions";
import { AddFunctionComponent } from "./add-function.component";
import { FunctionModel, FunctionRecord } from "./function.model";
import { UpdateFunctionConfigurationComponent } from "./update-configuration.component";

@Component({
  selector: 'function-panel',
  templateUrl: './function-panel.component.html',
  styleUrls: ['./function-panel.component.scss']
})
export class FunctionPanelComponent implements OnInit, OnDestroy {
  isLoading: boolean = false;
  private _model: FunctionModel | null = null;
  @Input()
  get model() : FunctionModel | null {
    return this._model;
  }
  set model(m: FunctionModel | null) {
    if (!m) {
      return;
    }

    this._model = m;
    this.refresh(m.content?.info?.name);
  }
  subscription: any;

  constructor(
    private dialog: MatDialog,
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute) { }

  ngOnInit() {
    this.subscription = this.store.pipe(select(fromReducers.selectFunctionResult)).subscribe((state: FunctionResult | null) => {
      if (!state || !this._model || !this._model.content || !this.isLoading) {
        return;
      }

      this.isLoading = false;
      this._model.content.info = state;
    });
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  chooseFunction() {
    const dialogRef = this.dialog.open(AddFunctionComponent, {
      width: '800px'
    });
    dialogRef.afterClosed().subscribe((opt: FunctionResult) => {
      if (!opt || !opt.name || !this.model) {
        return;
      }

      let record = new FunctionRecord();
      record.info = opt;
      this.model.content = record;
    });
  }

  updateConfiguration() {
    const dialogRef = this.dialog.open(UpdateFunctionConfigurationComponent, {
      data: this.model?.content,
      width: '800px'
    });
    dialogRef.afterClosed().subscribe((opt: any) => {
      if (!opt || !this.model || !this.model.content) {
        return;
      }

      this.model.content.configuration = opt;
    });
  }

  isSelected() {
    return this.model && this.model.content && this.model.content.info && this.model.content.info.name;
  }

  private refresh(name: string | undefined) {
    if (!name) {
      return;
    }

    this.isLoading = true;
    const action = startGet({ name: name });
    this.store.dispatch(action);
  }
}
