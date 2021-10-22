import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { startGet, startUpdateUIOperation } from '@stores/apis/actions/api.actions';
import { ApiDefinitionOperationResult, ApiDefinitionResult } from '@stores/apis/models/apidef.model';
import * as fromReducers from '@stores/appstate';
import { FunctionResult } from '@stores/functions/models/function.model';
import Drawflow from 'drawflow';
import { filter } from 'rxjs/operators';
import { BaseNodeModel } from './base-node.model';
import { FunctionModel, FunctionRecord } from './function-panel/function.model';
import { LaunchFunctionDialogComponent } from './launch-function-dialog.component';

declare var flowy: any;

class EditorModel {
  index: number = 0;
  model: BaseNodeModel<any> | null = null;
}

@Component({
  selector: 'edit-api',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss']
})
export class EditApiComponent implements OnInit, OnDestroy {
  editor: Drawflow | null = null;
  selectedNode: any | null = null;
  selectedIndex: number = 0;
  currentIndex: number = 0;
  nodes: EditorModel[] = [];
  name: string = "";
  opName: string = "";
  subscription: any;
  secondSubscription: any;
  mouseupHandler: any;
  isInitialized: boolean = false;
  currentOperation: ApiDefinitionOperationResult | null = null;

  constructor(
    private dialog: MatDialog,
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private actions$: ScannedActionsSubject,
    private snackBar: MatSnackBar,
    private translateService: TranslateService) { }

  ngOnInit() {
    this.mouseupHandler = this.handleMouseUp.bind(this);
    this.actions$.pipe(
      filter((action: any) => action.type === '[ApiDefs] COMPLETE_UPDATE_UI_OPERATION'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('apis.messages.apiOperationUpdated'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.refresh();
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[ApiDefs] ERROR_UPDATE_UI_OPERATION'))
      .subscribe(() => {
        this.snackBar.open(this.translateService.instant('apis.messages.errorUpdateApiOperation'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });

    this.nodes = [];
    this.refresh();
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }

    if (this.secondSubscription) {
      this.secondSubscription.unsubscribe();
    }

    window.removeEventListener('mouseup', this.mouseupHandler);
    this.destroy();
  }

  ngAfterViewInit() {
    var spacing_x = 20;
    var spacing_y = 50;
    window.addEventListener('mouseup', this.mouseupHandler);
    this.secondSubscription = this.store.pipe(select(fromReducers.selectApiDefResult)).subscribe((state: ApiDefinitionResult | null) => {
      if (!state || !this.opName || this.isInitialized) {
        return;
      }

      this.isInitialized = true;
      flowy(document.getElementById("canvas"), null, null, this.snapping.bind(this), null, spacing_x, spacing_y);
      this.currentOperation = state.operations.filter(o => o.name === this.opName)[0];
      if (this.currentOperation.ui) {
        flowy.import(this.currentOperation.ui);
        this.currentOperation.ui.blocks.forEach((b) => {
          const model = new FunctionModel(new FunctionRecord());
          const blockId = b.data.filter((d) => d.name === 'blockid')[0].value;
          if (b.model && model.content) {
            model.content.configuration = b.model.configuration;
          }

          if (b.model && b.model.info?.name && model.content) {
            model.content.info = new FunctionResult();
            model.content.info.name = b.model.info.name;
          }

          this.nodes.push({ index: parseInt(blockId), model: model });
        });
        const indexes: number[] = this.nodes.map(n => n.index);
        this.currentIndex = Math.max.apply(null, indexes) + 1;
      }
    });
  }

  snapping(b: any) {
    const self = this;
    const type = b.querySelector('.blockelemtype').value;
    b.setAttribute('data-index', self.currentIndex);
    b.setAttribute('data-type', type);
    switch (type) {
      case 'function':
        const data = new FunctionModel(new FunctionRecord());
        this.nodes.push({ index: self.currentIndex, model: data });
        break;
    }

    this.currentIndex++;
    return true;
  }


  close() {
    const node : any = document.querySelector("div[data-index='" + this.selectedIndex + "']");
    node.classList.remove('isselected');
    this.selectedNode = null;
    this.selectedIndex = 0;
  }

  launch() {
    this.dialog.open(LaunchFunctionDialogComponent, {
      width: '800px',
      data: {
        funcName: this.name,
        opName: this.currentOperation?.path
      }
    });
  }

  save() {
    const json = flowy.output();
    json.blocks.forEach((b : any) => {
      const blockId = b.data.filter((r : any) => r.name === 'blockid')[0].value;
      const filtered = this.nodes.filter((n) => n.index == parseInt(blockId));
      if (filtered.length === 1) {
        b.model = filtered[0].model?.content;
      }
    });

    json.html = json.html.replace('isselected', '');
    const name = this.activatedRoute?.snapshot.params['name'];
    const opName = this.activatedRoute?.snapshot.params['opname'];
    const updateUIOperation = startUpdateUIOperation({ funcName: name, operationName: opName, ui: json });
    this.store.dispatch(updateUIOperation);
  }

  private refresh() {
    this.name = this.activatedRoute?.snapshot.params['name'];
    this.opName = this.activatedRoute?.snapshot.params['opname'];
    const action = startGet({ funcName: this.name });
    this.store.dispatch(action);
  }

  private destroy() {
    if (flowy.destroy) {
      flowy.destroy();
    }
  }

  private handleMouseUp(evt: any) {
    const parent = evt.target.closest('.blockelem.block');
    if (!parent || parent.classList.contains('isselected')) {
      return;
    }

    const children: any = document.querySelectorAll('.blockelem.block');
    children.forEach(function (c: any) {
      c.classList.remove('isselected');
    });
    parent.classList.add('isselected');
    this.selectedIndex = parent.getAttribute('data-index');
    const filtered = this.nodes.filter((n) => n.index == this.selectedIndex);
    if (filtered.length === 0) {
      return;
    }

    this.selectedNode = filtered[0].model;
  }
}
