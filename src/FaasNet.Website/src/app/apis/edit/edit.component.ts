import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FunctionResult } from '@stores/functions/models/function.model';
import Drawflow from 'drawflow';
import { BaseNodeModel } from './base-node.model';
import { FunctionModel, FunctionRecord } from './function-panel/function.model';

declare var flowy: any;

class EditorModel {
  index: number = 0;
  model: BaseNodeModel<any> | null = null;
  elt: any | null = null;
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

  constructor(
    private dialog: MatDialog) { }

  ngOnInit() {
    this.nodes = [];
    const self = this;
    var spacing_x = 20;
    var spacing_y = 50;
    flowy(document.getElementById("canvas"), null, null, this.snapping.bind(this), null, spacing_x, spacing_y);
    window.addEventListener('mouseup', function (evt : any) {
      const parent = evt.target.closest('.blockelem.block');
      if (!parent || parent.classList.contains('isselected')) {
        return;
      }

      const children: any = document.querySelectorAll('.blockelem.block');
      children.forEach(function (c: any) {
        c.classList.remove('isselected');
      });
      parent.classList.add('isselected');
      self.selectedIndex = parent.getAttribute('data-index');
      const rec = self.nodes.filter((n) => n.index == self.selectedIndex)[0];
      self.selectedNode = rec.model;
    });
  }

  ngOnDestroy() {

  }

  snapping(b: any) {
    const self = this;
    const type = b.querySelector('.blockelemtype').value;
    b.setAttribute('data-index', self.currentIndex);
    b.setAttribute('data-type', type);
    switch (type) {
      case 'function':
        const data = new FunctionModel(new FunctionRecord());
        this.nodes.push({ index: self.currentIndex, model: data, elt: b });
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

  save() {
    const json = flowy.output();
    json.blocks.forEach((b : any) => {
      const blockId = b.data.filter((r : any) => r.name === 'blockid')[0].value;
      const filtered = this.nodes.filter((n) => n.index == parseInt(blockId));
      if (filtered.length === 1) {
        b.model = filtered[0].model?.content;
      }
    });

    console.log(json);
  }
}
