import { Component, Input, OnDestroy, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatMenuTrigger } from '@angular/material/menu';
import { StateMachineInstanceDetails } from '@stores/statemachineinstances/models/statemachineinstance-details.model';
import { ForeachStateMachineState } from '@stores/statemachines/models/statemachine-foreach-state.model';
import { InjectStateMachineState } from '@stores/statemachines/models/statemachine-inject-state.model';
import { OperationStateMachineState } from '@stores/statemachines/models/statemachine-operation-state.model';
import { BaseTransition, EmptyTransition, StateMachineState } from '@stores/statemachines/models/statemachine-state.model';
import { SwitchStateMachineState } from '@stores/statemachines/models/statemachine-switch-state.model';
import { StateMachineModel } from '@stores/statemachines/models/statemachinemodel.model';
import { BehaviorSubject } from 'rxjs';
import { TokenComponent } from './components/token/token.component';

class DiagramNode {
  constructor(public x: number, public y: number, public level: number, public state: StateMachineState | undefined) {

  }

  public status: string = "";

  get transform() {
    return "translate(" + this.x + "," + this.y + ")";
  }

  public computePosition(position: number, nbNodes: number, nodeGap: number, nodeWidth: number, nodeHeigth : number, fullWidth: number): void {
    const middle = fullWidth / 2;
    let nbHalfNodes = Math.floor(nbNodes / 2);
    const fullNodeWidth = nodeWidth + nodeGap;
    if (nbNodes === 1) {
      this.x = middle - (nodeWidth / 2);
    } else {
      let min = middle - (fullNodeWidth * nbHalfNodes);
      let newPosition = min + (fullNodeWidth * position) - (nodeWidth / 2);
      this.x = newPosition;
    }

    this.y = (this.level * (nodeHeigth + nodeGap)) + nodeGap;
  }
}

class DiagramNodeToken {
  constructor(public data: any, public isInput: boolean, public node: DiagramNode) { }

  posX: number = 0;
  posY: number = 0;

  public computePosition(tokenWidth: number, tokenHeight: number, nodeHeight: number) {
    this.posX = this.node.x - (tokenWidth / 2);
    this.posY = this.node.y + (this.isInput ? 0 : (nodeHeight - tokenHeight / 2));
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

  public isDefault() {
    return this.transition.getType() == EmptyTransition.TYPE && this.formNode.state?.type == SwitchStateMachineState.TYPE;
  }

  public getMarkerEnd() {
    return 'url(#' + (this.isDefault() ? 'statediagram-barbEndDefault' : 'statediagram-barbEnd') +')';
  }
}

class EdgeLabel {
  constructor(public label: BehaviorSubject<string>, public edgePath: EdgePath) {

  }

  posX: number = 0;
  posY: number = 0;
  width: number = 0;
  height: number = 0;
  selected: boolean = false;

  public computePosition() {
    let edgePathWidth = this.edgePath.fromX - this.edgePath.toX;
    let edgePathHeight = this.edgePath.toY - this.edgePath.fromY;
    if (edgePathWidth < 0) {
      edgePathWidth = -edgePathWidth;
    }

    if (this.width === 0) {
      this.width = 100;
    }

    if (this.height === 0) {
      this.height = 30;
    }

    const fromX = (this.edgePath.fromX > this.edgePath.toX) ? this.edgePath.toX : this.edgePath.fromX;
    this.posX = fromX + (edgePathWidth / 2) - (this.width / 2);
    this.posY = this.edgePath.fromY + (edgePathHeight / 2) - (this.height / 2);
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
  canvasWidth: number = 800;
  canvasHeight: number = 600;
  nodeGap: number = 50;
  nodeWidth: number = 200;
  nodeHeight: number = 90;
  tokenWidth: number = 100;
  tokenHeight: number = 30;
  zoom: number = 20;
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
  private _stateMachine: StateMachineModel = new StateMachineModel();
  private _stateMachineInstance: StateMachineInstanceDetails = new StateMachineInstanceDetails();
  @Input()
  get stateMachine(): StateMachineModel {
    return this._stateMachine;
  }
  set stateMachine(s: StateMachineModel) {
    this._stateMachine = s;
    if (s) {
      this.refreshUI();
    }
  }
  @Input()
  get stateMachineInstance(): StateMachineInstanceDetails {
    return this._stateMachineInstance;
  }
  set stateMachineInstance(s: StateMachineInstanceDetails) {
    this._stateMachineInstance = s;
    if (s) {
      this.refreshUI();
    }
  }
  @Input() options: DiagramOptions = new DiagramOptions();
  @Input() editMode: boolean = true;
  @ViewChild("stateDiagram") stateDiagram: any;
  @ViewChild("gutter") gutter: any;
  @ViewChild("stateDiagramContainer") stateDiagramContainer: any;
  @ViewChildren(MatMenuTrigger) contextMenu: QueryList<MatMenuTrigger> | null = null;
  circleStartPosition: { x: number, y: number } = { x: 0, y: 0 };
  circleStartSelected: boolean = false;
  edgePathCircleStart: string = "";
  viewBox: string = "0 0 1 1";
  isMoving: boolean = false;
  isResizing: boolean = false;
  startMoving: boolean = false;
  previousMousePosition: { x: number, y: number } = { x: 0, y: 0 };
  nodes: DiagramNode[] = [];
  edgePaths: EdgePath[] = [];
  edgeLabels: EdgeLabel[] = [];
  anchors: Anchor[] = [];
  draggableZoneLst: DraggableZone[] = [];
  tokens: DiagramNodeToken[] = [];
  selectedState: StateMachineState | null = null;
  selectedTransition: BaseTransition | null = null;
  handleMouseMoveWindowRef: any | null = null;
  handleMouseUpRef: any | null = null;
  gutterBoundingRect: any | null = null;

  constructor(
    private dialog: MatDialog) {
  }

  ngOnInit() {
    this.handleMouseMoveWindowRef = this.handleMouseMoveWindow.bind(this);
    this.handleMouseUpRef = this.handleMouseUp.bind(this);
  }

  ngOnDestroy() {
    if (this.handleMouseMoveWindowRef) {
      window.removeEventListener('mousemove', this.handleMouseMoveWindowRef);
    }
  }

  ngAfterViewInit() {
    this.viewBox = "0 0 " + this.stateDiagramContainer.nativeElement.offsetWidth + " " + this.stateDiagramContainer.nativeElement.offsetHeight;
    this.initListener();
  }

  closePanel() {
    this.selectedState = null;
    this.selectedTransition = null;
  }

  onDragStart(evt: any, type: string) {
    evt.dataTransfer.setData('type', type);
  }

  onDrop(evt: any, draggableZone: DraggableZone) {
    if (this.startMoving) {
      return;
    }

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
      case ForeachStateMachineState.TYPE:
        child = new ForeachStateMachineState();
        break;
      case OperationStateMachineState.TYPE:
        child = new OperationStateMachineState();
        break;
    }

    if (child) {
      child.id = this.buildId();
      child.name = type;
      child.end = true;
      const removedTransition = parent?.tryAddTransition(child.id);
      if (removedTransition) {
        this.stateMachine?.removeByNames([removedTransition]);
      }

      this.stateMachine?.states.push(child);
      this.refreshUI();
      this.closePanel();
    }
  }

  onDragOver(evt: any, draggableZone: DraggableZone) {
    if (this.startMoving) {
      return;
    }

    evt.preventDefault();
    draggableZone.anchor.selected = true;
  }

  onDragOverCircleStart(evt: any) {
    if (this.startMoving) {
      return;
    }

    this.circleStartSelected = true;
    evt.preventDefault();
  }

  onDragLeaveCircleStart(evt: any) {
    if (this.startMoving) {
      return;
    }

    this.circleStartSelected = false;
  }

  onDropCircleStart(evt: any) {
    if (this.startMoving) {
      this.circleStartSelected = false;
      return;
    }

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
      case ForeachStateMachineState.TYPE:
        child = new ForeachStateMachineState();
        break;
      case OperationStateMachineState.TYPE:
        child = new OperationStateMachineState();
        break;
    }

    if (child) {
      this.stateMachine.states = [];
      child.id = this.buildId();
      child.name = type;
      child.end = true;
      if (this.stateMachine.start) {
        this.stateMachine.start.stateName = child.id;
      }

      this.stateMachine?.states.push(child);
      this.refreshUI();
      this.closePanel();
    }
  }

  onDragLeave(evt: any, draggableZone: DraggableZone) {
    if (this.startMoving) {
      return;
    }

    draggableZone.anchor.selected = false;
  }

  removeNode(node: DiagramNode) {
    if (this.startMoving) {
      return;
    }

    const state = node.state;
    if (!state) {
      return;
    }

    let stateNextTransitions = state.getNextTransitions();
    let parent = this.getParent(state);
    if (!parent) {
      if (!stateNextTransitions || stateNextTransitions.length === 0) {
        this.stateMachine?.remove(state);
        this.refreshUI();
        return;
      }
    }

    let parentNextTransitions = parent?.getNextTransitions();
    parentNextTransitions = parentNextTransitions?.filter((t) => t.transition !== state?.id);
    this.stateMachine?.remove(state);
    if (parentNextTransitions) {
      parent?.setTransitions(parentNextTransitions);
    }

    this.refreshUI();
    this.closePanel();
  }

  selectState(node: DiagramNode) {
    if (this.startMoving || !node.state) {
      return;
    }

    this.selectedState = node.state;
    this.edgeLabels.forEach((e) => {
      e.selected = false;
    });
    this.selectedTransition = null;
  }

  zoomIn() {
    this.zoom(-(this.options.zoom));
  }

  zoomOut() {
    this.zoom(this.options.zoom);
  }

  startMove() {
    this.startMoving = true;
  }

  startEdit() {
    this.startMoving = false;
  }

  selectLabel(label: EdgeLabel) {
    this.edgeLabels.forEach((e) => {
      e.selected = false;
    });
    this.selectedState = null;
    label.selected = !label.selected;
    this.selectedTransition = label.edgePath.transition;
  }

  onContextMenu(evt: MouseEvent, edgeLabel: EdgeLabel, index : number) {
    evt.preventDefault();
    if (edgeLabel.edgePath.transition.getType() == "empty") {
      return;
    }

    const matMenuTrigger = this.contextMenu?.get(index);
    matMenuTrigger?.openMenu();
  }

  displayToken(token: DiagramNodeToken) {
    this.dialog.open(TokenComponent, {
      data: token.data,
      width: '800px'
    });
  }

  setDefaultCondition(edgeLabel: EdgeLabel) {
    const switchStateMachine = edgeLabel.edgePath.formNode.state as SwitchStateMachineState;
    switchStateMachine.swichTransitionToDefault(edgeLabel.edgePath.transition);
    this.refreshUI();
    this.selectedTransition = null;
  }

  private initListener() {
    this.initStateDiagramListeners();
  }

  private initStateDiagramListeners() {
    const self = this;
    const native = this.stateDiagram.nativeElement;
    const viewBox = native.viewBox;
    native.onmousedown = function (e: any) {
      if (!self.startMoving) {
        return;
      }

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

  private handleMouseMoveWindow(e: any) {
    if (!this.isResizing) {
      return;
    }

    const stateDiagramContainer = this.stateDiagramContainer.nativeElement;
    const newX = this.previousMousePosition.x - e.x;
    stateDiagramContainer.style.width = this.gutterBoundingRect.width - newX + "px";
  }

  private handleMouseUp(e: any) {
    if (this.isResizing) {
      this.isResizing = false;
    }

    if (this.isMoving) {
      this.isMoving = false;
    }
  }

  private zoom(delta: number) {
    const native = this.stateDiagram.nativeElement;
    const viewBox = native.viewBox;
    const w = viewBox.animVal.width;
    const h = viewBox.animVal.height;
    this.viewBox = (viewBox.animVal.x) + " " +(viewBox.animVal.y)+ " " + (w + delta) + " " + (h + delta);
  }

  private refreshUI() {
    if (!this._stateMachine.id) {
      return;
    }

    this.circleStartPosition = { x: this.computeCirclePosition(), y: 10 };
    this.nodes = [];
    this.edgePaths = [];
    this.edgeLabels = [];
    this.anchors = [];
    this.draggableZoneLst = [];
    this.tokens = [];
    const rootState = this.stateMachine?.getRootState();
    if (!rootState) {
      return;
    }

    this.buildNodeHierarchy(rootState, undefined);
    this.updateNodePosition();
    this.updateStartAndEndEdgePath();
  }

  private buildNodeHierarchy(state: StateMachineState, parentNode: DiagramNode | undefined): DiagramNode {
    const self = this;
    let newNode: DiagramNode = new DiagramNode(0, 0, 0, state);
    if (parentNode) {
      newNode.level = parentNode.level + 1;
    }

    let existingNodes = this.nodes.filter((n: DiagramNode) => n.state?.id === state.id);
    if (existingNodes.length === 1) {
      const existingNode = existingNodes[0];
      if (existingNode.level < newNode.level) {
        existingNode.level = newNode.level;
      }

      return existingNode;
    }

    if (self._stateMachineInstance && self._stateMachineInstance.states) {
      let filteredStateInstances = self._stateMachineInstance.states.filter((s) => {
        return s.defId === newNode.state?.id;
      });
      if (filteredStateInstances.length === 1) {
        const instance = filteredStateInstances[0];
        newNode.status = instance.status;
        if (instance.input) {
          this.tokens.push(new DiagramNodeToken(instance.input, true, newNode));
        }

        if (instance.output) {
          this.tokens.push(new DiagramNodeToken(instance.output, false, newNode));
        }
      }
    }
    this.nodes.push(newNode);
    const anchor = new Anchor(newNode);
    const draggableZone = new DraggableZone(newNode, anchor);
    this.anchors.push(anchor);
    this.draggableZoneLst.push(draggableZone);
    const nextNodeNames = state.getNextTransitions();
    state.updated.subscribe(() => {
      self.refreshUI();
    });
    nextNodeNames.forEach((transition: BaseTransition) => {
      const child = this.stateMachine?.states.filter((s: StateMachineState) => s.id === transition.transition)[0];
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
    if (!totalWidth) {
      return;
    }

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

    this.tokens.forEach((t: DiagramNodeToken) => {
      t.computePosition(this.options.tokenWidth, this.options.tokenHeight, this.options.nodeHeight);
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
    if (!totalWidth) {
      return 0;
    }

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
      return e.targetNode.state?.id == state.id;
    });
    return filtered.length === 1 ? filtered[0].formNode.state : null;
  }

  private buildId() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        var r = Math.random()*16|0, v = c == 'x' ? r : (r&0x3|0x8);
        return v.toString(16);
    });
  }


  private getSvgWidth() {
    if (!this.stateDiagram) {
      return null;
    }

    const native = this.stateDiagram.nativeElement;
    return native.width.baseVal.value;
  }
}
