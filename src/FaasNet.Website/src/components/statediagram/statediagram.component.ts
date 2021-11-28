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

class EdgePath {
  constructor(public formNode: DiagramNode, public targetNode: DiagramNode) { }

  path: string = "";

  public computePosition(nodeWidth: number, nodeHeight: number) {
    const formX = this.formNode.x + (nodeWidth / 2);
    const formY = this.formNode.y + nodeHeight;
    const toX = this.targetNode.x + (nodeWidth / 2);
    const toY = this.targetNode.y;
    this.path = "M" + formX + ","  + formY + "L" + toX + "," + toY;
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
  @Input() stateMachine: StateMachine | undefined;
  @Input() options: DiagramOptions = new DiagramOptions();
  @ViewChild("stateDiagram") stateDiagram: any;
  handleResizeRef: any;
  circleStartPosition: { x: number, y: number } = { x: 0, y: 0 };
  edgePathCircleStart: string = "";
  nodes: DiagramNode[] = [];
  edgePaths: EdgePath[] = [];
  anchors: Anchor[] = [];
  draggableZoneLst: DraggableZone[] = [];

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

  onDragStart(evt: any, type: string) {
    evt.dataTransfer.setData('type', type);
  }

  onDrop(evt: any, draggableZone: DraggableZone) {
    draggableZone.anchor.selected = false;
    const type = evt.dataTransfer.getData('type');
    switch (type) {
      case 'inject':
        const parent = draggableZone.diagramNode.state as InjectStateMachineState;
        const child = new InjectStateMachineState();
        child.name = this.uuidv4();
        child.transition = parent.transition;
        if (child.name) {
          parent.transition = child.name;
        }

        this.stateMachine?.states.push(child);
        this.handleResize();
        console.log(this.stateMachine?.states);
        break;
    }
  }

  onDragOver(evt: any, draggableZone: DraggableZone) {
    evt.preventDefault();
    draggableZone.anchor.selected = true;
  }

  onDragLeave(evt: any, draggableZone: DraggableZone) {
    draggableZone.anchor.selected = false;
  }

  private handleResize() {
    this.circleStartPosition = { x: this.computeCirclePosition(), y: 10 };
    this.nodes = [];
    this.edgePaths = [];
    this.anchors = [];
    this.draggableZoneLst = [];
    const rootState = this.stateMachine?.states[0];
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
    const nextNodeNames = this.getNextNodes(state);
    nextNodeNames.forEach((name: string) => {
      const child = this.stateMachine?.states.filter((s: StateMachineState) => s.name === name)[0];
      if (!child) {
        return;
      }

      const nodeChild = this.buildNodeHierarchy(child, newNode);
      const edgePath = new EdgePath(newNode, nodeChild);
      this.edgePaths.push(edgePath);
    });

    return newNode;
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

    this.edgePaths.forEach((ep: EdgePath) => {
      ep.computePosition(this.options.nodeWidth, this.options.nodeHeight);
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
    return (totalWidth / 2);
  }

  private groupBy(xs: any, key: any) {
    return xs.reduce(function (rv: any, x: any) {
      (rv[x[key]] = rv[x[key]] || []).push(x);
      return rv;
    }, {});
  }

  private uuidv4() {
    var dt = new Date().getTime();
    var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
      var r = (dt + Math.random() * 16) % 16 | 0;
      dt = Math.floor(dt / 16);
      return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
    return uuid;
  }
}
