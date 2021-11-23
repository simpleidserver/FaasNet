import { Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { InjectStateMachineState } from './models/statemachine-inject-state.model';
import { StateMachineState } from './models/statemachine-state.model';
import { StateMachine } from './models/statemachine.model';

class DiagramNode {
  constructor(public x: number, public y: number, public level: number, public state: StateMachineState | undefined) {

  }

  get transform() {
    return "translate(" + this.x + "," + this.y + ")";
  }

  public computePosition(position: number, nbNodes: number, nodeGap: number, nodeWidth: number, nodeHeigth : number, fullWidth: number): void {
    const cellWidth = fullWidth / nbNodes;
    this.x = (cellWidth * position) + ((cellWidth / 2) - (nodeWidth / 2));
    this.y = (this.level * (nodeHeigth + nodeGap)) + nodeGap;
  }
}

class DiagramOptions {
  nodeGap: number = 50;
  nodeWidth: number = 200;
  nodeHeight: number = 90;
  circleRadius: number = 7;
  get titleHeight() {
    return this.nodeHeight * 0.20;
  }

  get bodyHeight() {
    return this.nodeHeight * 0.8;
  }
}

@Component({
  selector: 'state-diagram',
  templateUrl: './statediagram.component.html',
  styleUrls: ['./statediagram.component.scss']
})
export class StateDiagramComponent implements OnInit, OnDestroy {
  @Input() stateMachine: StateMachine | undefined;
  @Input() options: DiagramOptions = new DiagramOptions();
  @ViewChild("stateDiagram") stateDiagram: any;
  handleResizeRef: any;
  circleStartPosition: { x: number, y: number } = { x: 0, y: 0 };
  nodes: DiagramNode[] = [];

  constructor() {
    this.handleResizeRef = this.handleResize.bind(this);
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    window.removeEventListener('resize', this.handleResizeRef);
  }

  ngAfterViewInit() {
    window.addEventListener('resize', this.handleResizeRef);
    this.handleResize();
  }

  private handleResize() {
    this.circleStartPosition = { x: this.computeCirclePosition(), y: 10 };
    this.nodes = [];
    const rootState = this.stateMachine?.states[0];
    if (!rootState) {
      return;
    }

    this.buildNodeHierarchy(rootState, undefined);
    this.updateNodePosition();
  }

  private buildNodeHierarchy(state: StateMachineState, parentNode: DiagramNode | undefined) {
    let newNode: DiagramNode = new DiagramNode(0, 0, 0, state);
    if (parentNode) {
      newNode.level = parentNode.level + 1;
    }

    let existingNodes = this.nodes.filter((n: DiagramNode) => n.state?.name === state.name);
    if (existingNodes.length === 1) {
      const existingNode = existingNodes[0];
      if (existingNode.level < newNode.level) {
        existingNode.level = newNode.level;
      }

      return;
    }

    this.nodes.push(newNode);
    const nextNodeNames = this.getNextNodes(state);
    nextNodeNames.forEach((name: string) => {
      const child = this.stateMachine?.states.filter((s: StateMachineState) => s.name === name)[0];
      if (!child) {
        return;
      }

      this.buildNodeHierarchy(child, newNode);
    });
  }

  private updateNodePosition() {
    const totalWidth = this.stateDiagram.nativeElement.offsetWidth;
    const groupedResult = this.groupBy(this.nodes, 'level');
    for (var key in groupedResult) {
      let index: number = 0;
      const nbNodes = groupedResult[key].length;
      groupedResult[key].forEach((n: DiagramNode) => {
        n.computePosition(index, nbNodes, this.options.nodeGap, this.options.nodeWidth, this.options.nodeHeight, totalWidth);
        index++;
      });
    }
  }

  private getNextNodes(state: StateMachineState) : string[] {
    switch (state.type) {
      case "inject":
        var injectState = state as InjectStateMachineState;
        return [injectState.transition];
    }

    return [];
  }

  private computeCirclePosition() : number {
    const totalWidth = this.stateDiagram.nativeElement.offsetWidth;
    return (totalWidth / 2) - (this.options.circleRadius / 2);
  }

  private groupBy(xs: any, key: any) {
    return xs.reduce(function (rv: any, x: any) {
      (rv[x[key]] = rv[x[key]] || []).push(x);
      return rv;
    }, {});
  };
}
