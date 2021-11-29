import { Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { InjectStateMachineState } from './models/statemachine-inject-state.model';
import { BaseTransition, StateMachineState } from './models/statemachine-state.model';
import { SwitchStateMachineState } from './models/statemachine-switch-state.model';
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

class EdgePath {
  constructor(public formNode: DiagramNode, public targetNode: DiagramNode, public transition: BaseTransition) { }

  path: string = "";
  fromX: number = 0;
  fromY: number = 0;
  toX: number = 0;
  toY: number = 0;

  public computePosition(nodeWidth: number, nodeHeight: number) {
    this.fromX = this.formNode.x + (nodeWidth / 2);
    this.fromY = this.formNode.y + nodeHeight;
    this.toX = this.targetNode.x + (nodeWidth / 2);
    this.toY = this.targetNode.y;
    this.path = "M" + this.fromX + ","  + this.fromY + "L" + this.toX + "," + this.toY;
  }
}

class EdgeLabel {
  constructor(public label: string, public edgePath: EdgePath) {

  }

  posX: number = 0;
  posY: number = 0;
  width: number = 0;
  height: number = 0;

  public computePosition() {
    this.posX = (this.edgePath.fromX > this.edgePath.toX) ? this.edgePath.toX : this.edgePath.fromX;
    this.posY = this.edgePath.fromY;
    this.width = this.edgePath.fromX - this.edgePath.toX;
    if (this.width < 0) {
      this.width = -this.width;
    }

    this.height = this.edgePath.toY - this.edgePath.fromY;
  }
}

class Anchor {
  constructor(public diagramNode: DiagramNode) { }

  x: number = 0;
  y: number = 0;
  selected: boolean = false;

  get transform() {
    return "translate(" + this.x + "," + this.y + ")";
  }

  public computePosition(nodeWidth: number, nodeHeight: number) {
    this.x = this.diagramNode.x + (nodeWidth / 2);
    this.y = this.diagramNode.y + nodeHeight;
  }
}

class DraggableZone {
  constructor(public diagramNode: DiagramNode, public anchor: Anchor) { }

  x: number = 0;
  y: number = 0;
  width: number = 0;
  height: number = 0;

  public computPosition(nodeWidth: number, nodeHeight: number, nodeGap: number, draggableZoneWidth: number) {
    this.x = this.diagramNode.x + (nodeWidth / 2) - (draggableZoneWidth / 2);
    this.y = this.diagramNode.y + (nodeHeight);
    this.width = draggableZoneWidth;
    this.height = nodeGap;
  }
}

class DiagramOptions {
  canvasWidth: number = 1000;
  canvasHeight: number = 600;
  nodeGap: number = 50;
  nodeWidth: number = 200;
  nodeHeight: number = 90;
  draggableZoneWidth: number = 30;
  circleRadius: number = 7;
  get titleHeight() {
    return this.nodeHeight * 0.30;
  }

  get bodyHeight() {
    return this.nodeHeight * 0.7;
  }
}

@Component({
  selector: 'state-diagram',
  templateUrl: './statediagram.component.html',
  styleUrls: ['./statediagram.component.scss']
})
export class StateDiagramComponent implements OnInit, OnDestroy {
  @Input() stateMachine: StateMachine | null = null;
  @Input() options: DiagramOptions = new DiagramOptions();
  @ViewChild("stateDiagram") stateDiagram: any;
  circleStartPosition: { x: number, y: number } = { x: 0, y: 0 };
  circleStartSelected: boolean = false;
  edgePathCircleStart: string = "";
  viewBox: string = "";
  isMoving: boolean = false;
  previousMousePosition: { x: number, y: number } = {x : 0, y : 0};
  nodes: DiagramNode[] = [];
  edgePaths: EdgePath[] = [];
  edgeLabels: EdgeLabel[] = [];
  anchors: Anchor[] = [];
  draggableZoneLst: DraggableZone[] = [];
  handleZoomRef: any;

  constructor() {
  }

  ngOnInit() {
  }

  ngOnDestroy() {
  }

  ngAfterViewInit() {
    this.refreshUI();
    this.viewBox = "0 0 " + this.options.canvasWidth + " " + this.options.canvasHeight;
    this.handleZoomRef = this.handleZoom.bind(this);
    this.initListener();
  }

  onDragStart(evt: any, type: string) {
    evt.dataTransfer.setData('type', type);
  }

  onDrop(evt: any, draggableZone: DraggableZone) {
    draggableZone.anchor.selected = false;
    const parent = draggableZone.diagramNode.state;
    let child: StateMachineState | null = null;
    const type = evt.dataTransfer.getData('type');
    switch (type) {
      case InjectStateMachineState.TYPE:
        child = new InjectStateMachineState();
        break;
      case SwitchStateMachineState.TYPE:
        child = new SwitchStateMachineState();
        break;
    }

    if (child) {
      child.name = this.buildId(type);
      const removedTransitions = parent?.setTransitions([child.name]);
      if (removedTransitions) {
        this.stateMachine?.removeByNames(removedTransitions);
      }

      this.stateMachine?.states.push(child);
      this.refreshUI();
    }
  }

  onDragOver(evt: any, draggableZone: DraggableZone) {
    evt.preventDefault();
    draggableZone.anchor.selected = true;
  }

  onDragOverCircleStart(evt : any) {
    this.circleStartSelected = true;
    evt.preventDefault();
  }

  onDragLeaveCircleStart(evt: any) {
    this.circleStartSelected = false;
  }

  onDropCircleStart(evt: any) {
    this.circleStartSelected = false;
    let child: StateMachineState | null = null;
    const type = evt.dataTransfer.getData('type');
    switch (type) {
      case InjectStateMachineState.TYPE:
        child = new InjectStateMachineState();
        break;
      case SwitchStateMachineState.TYPE:
        child = new SwitchStateMachineState();
        break;
    }

    const rootState = this.stateMachine?.getRootState();
    if (child) {
      child.name = this.buildId(type);
      if (rootState && rootState.name) {
        child.setTransitions([rootState.name]);
      }

      this.stateMachine?.states.push(child);
      this.refreshUI();
    }
  }

  onDragLeave(evt: any, draggableZone: DraggableZone) {
    draggableZone.anchor.selected = false;
  }

  removeNode(node: DiagramNode) {
    const state = node.state;
    if (!state) {
      return;
    }

    let nextTransitions = state.getNextTransitions();
    let parent = this.getParent(state);
    if (!parent) {
      if (!nextTransitions || nextTransitions.length === 0) {
        this.stateMachine?.remove(state);
        return;
      }

      parent = this.stateMachine?.getState(nextTransitions[0].transition);
      nextTransitions = nextTransitions.filter((t) => t.transition !== parent?.name);
    }

    this.stateMachine?.remove(state);
    parent?.setTransitions(nextTransitions.map((t) => t.transition));
    this.refreshUI();
  }

  openSettings(node: DiagramNode) {
    console.log(node);
  }

  private initListener() {
    const self = this;
    const native = this.stateDiagram.nativeElement;
    const viewBox = native.viewBox;
    native.onmousewheel = this.handleZoomRef;
    native.onmousedown = function (e : any) {
      self.isMoving = true;
      self.previousMousePosition = { x: e.clientX + viewBox.animVal.x, y: e.clientY + viewBox.animVal.y };
    };
    native.onmousemove = function (e: any) {
      if (!self.isMoving) {
        return;
      }

      const diffX = -(e.clientX - self.previousMousePosition.x);
      const diffY = -(e.clientY - self.previousMousePosition.y);
      self.viewBox = diffX + " " + diffY + " " + viewBox.animVal.width + " " + viewBox.animVal.height;
    };
    native.onmouseup = function (e: any) {
      self.isMoving = false;
    };
  }

  private handleZoom(evt: any) {
    const native = this.stateDiagram.nativeElement;
    evt.preventDefault();
    const viewBox = native.viewBox;
    const w = viewBox.animVal.width;
    const h = viewBox.animVal.height;
    const dw = w * Math.sign(evt.deltaY) * 0.05;
    const dh = h * Math.sign(evt.deltaY) * 0.05;
    this.viewBox = (viewBox.animVal.x) + " " +(viewBox.animVal.y)+ " " + (w - dw) + " " + (h - dh);
  }

  private refreshUI() {
    this.circleStartPosition = { x: this.computeCirclePosition(), y: 10 };
    this.nodes = [];
    this.edgePaths = [];
    this.edgeLabels = [];
    this.anchors = [];
    this.draggableZoneLst = [];
    const rootState = this.stateMachine?.getRootState();
    if (!rootState) {
      return;
    }

    this.buildNodeHierarchy(rootState, undefined);
    this.updateNodePosition();
    this.updateStartAndEndEdgePath();
  }

  private buildNodeHierarchy(state: StateMachineState, parentNode: DiagramNode | undefined) : DiagramNode{
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

      return existingNode;
    }

    this.nodes.push(newNode);
    const anchor = new Anchor(newNode);
    const draggableZone = new DraggableZone(newNode, anchor);
    this.anchors.push(anchor);
    this.draggableZoneLst.push(draggableZone);
    const nextNodeNames = state.getNextTransitions();
    nextNodeNames.forEach((transition: BaseTransition) => {
      const child = this.stateMachine?.states.filter((s: StateMachineState) => s.name === transition.transition)[0];
      if (!child) {
        return;
      }

      const nodeChild = this.buildNodeHierarchy(child, newNode);
      const edgePath = new EdgePath(newNode, nodeChild, transition);
      const label = transition.getLabel();
      if (label) {
        const edgeLabel = new EdgeLabel(label, edgePath);
        this.edgeLabels.push(edgeLabel);
      }

      this.edgePaths.push(edgePath);
    });

    return newNode;
  }

  private updateNodePosition() {
    const totalWidth = this.getSvgWidth();
    const groupedResult = this.groupBy(this.nodes, 'level');
    for (var key in groupedResult) {
      let index: number = 0;
      const nbNodes = groupedResult[key].length;
      groupedResult[key].forEach((n: DiagramNode) => {
        n.computePosition(index, nbNodes, this.options.nodeGap, this.options.nodeWidth, this.options.nodeHeight, totalWidth);
        index++;
      });
    }

    this.edgePaths.forEach((ep: EdgePath) => {
      ep.computePosition(this.options.nodeWidth, this.options.nodeHeight);
    });

    this.edgeLabels.forEach((el: EdgeLabel) => {
      el.computePosition();
    });

    this.anchors.forEach((a: Anchor) => {
      a.computePosition(this.options.nodeWidth, this.options.nodeHeight);
    });

    this.draggableZoneLst.forEach((d: DraggableZone) => {
      d.computPosition(this.options.nodeWidth, this.options.nodeHeight, this.options.nodeGap, this.options.draggableZoneWidth);
    });
  }

  private updateStartAndEndEdgePath() {
    if (this.nodes.length === 0) {
      return;
    }

    const formX = this.circleStartPosition.x;
    const formY = this.circleStartPosition.y;
    const toX = this.nodes[0].x + (this.options.nodeWidth / 2);
    const toY = this.nodes[0].y;
    this.edgePathCircleStart = "M" + formX + "," + formY + "L" + toX + "," + toY;
  }

  private computeCirclePosition() : number {
    const totalWidth = this.getSvgWidth();
    return (totalWidth / 2);
  }

  private groupBy(xs: any, key: any) {
    return xs.reduce(function (rv: any, x: any) {
      (rv[x[key]] = rv[x[key]] || []).push(x);
      return rv;
    }, {});
  }

  private getParent(state : StateMachineState) {
    const filtered =  this.edgePaths.filter((e: EdgePath) => {
      return e.targetNode.state?.name == state.name;
    });
    return filtered.length === 1 ? filtered[0].formNode.state : null;
  }

  private buildId(type: string) {
    const date = new Date();
    return type + "-" + date.getTime();
  }

  private getSvgWidth() {
    const native = this.stateDiagram.nativeElement;
    return native.width.baseVal.value;
  }
}
