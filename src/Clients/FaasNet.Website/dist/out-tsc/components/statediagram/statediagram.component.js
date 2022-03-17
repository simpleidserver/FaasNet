import { __decorate } from "tslib";
import { Component, Input, ViewChild, ViewChildren } from '@angular/core';
import { MatMenuTrigger } from '@angular/material/menu';
import { StateMachineInstanceDetails } from '@stores/statemachineinstances/models/statemachineinstance-details.model';
import { ForeachStateMachineState } from '@stores/statemachines/models/statemachine-foreach-state.model';
import { InjectStateMachineState } from '@stores/statemachines/models/statemachine-inject-state.model';
import { OperationStateMachineState } from '@stores/statemachines/models/statemachine-operation-state.model';
import { EmptyTransition } from '@stores/statemachines/models/statemachine-state.model';
import { SwitchStateMachineState } from '@stores/statemachines/models/statemachine-switch-state.model';
import { StateMachineModel } from '@stores/statemachines/models/statemachinemodel.model';
import { TokenComponent } from './components/token/token.component';
class DiagramNode {
    constructor(x, y, level, state) {
        this.x = x;
        this.y = y;
        this.level = level;
        this.state = state;
        this.status = "";
    }
    get transform() {
        return "translate(" + this.x + "," + this.y + ")";
    }
    computePosition(position, nbNodes, nodeGap, nodeWidth, nodeHeigth, fullWidth) {
        const middle = fullWidth / 2;
        let nbHalfNodes = Math.floor(nbNodes / 2);
        const fullNodeWidth = nodeWidth + nodeGap;
        if (nbNodes === 1) {
            this.x = middle - (nodeWidth / 2);
        }
        else {
            let min = middle - (fullNodeWidth * nbHalfNodes);
            let newPosition = min + (fullNodeWidth * position) - (nodeWidth / 2);
            this.x = newPosition;
        }
        this.y = (this.level * (nodeHeigth + nodeGap)) + nodeGap;
    }
}
class DiagramNodeToken {
    constructor(data, isInput, node) {
        this.data = data;
        this.isInput = isInput;
        this.node = node;
        this.posX = 0;
        this.posY = 0;
    }
    computePosition(tokenWidth, tokenHeight, nodeHeight) {
        this.posX = this.node.x - (tokenWidth / 2);
        this.posY = this.node.y + (this.isInput ? 0 : (nodeHeight - tokenHeight / 2));
    }
}
class EdgePath {
    constructor(formNode, targetNode, transition) {
        this.formNode = formNode;
        this.targetNode = targetNode;
        this.transition = transition;
        this.path = "";
        this.fromX = 0;
        this.fromY = 0;
        this.toX = 0;
        this.toY = 0;
    }
    computePosition(nodeWidth, nodeHeight) {
        this.fromX = this.formNode.x + (nodeWidth / 2);
        this.fromY = this.formNode.y + nodeHeight;
        this.toX = this.targetNode.x + (nodeWidth / 2);
        this.toY = this.targetNode.y;
        this.path = "M" + this.fromX + "," + this.fromY + "L" + this.toX + "," + this.toY;
    }
    isDefault() {
        var _a;
        return this.transition.getType() == EmptyTransition.TYPE && ((_a = this.formNode.state) === null || _a === void 0 ? void 0 : _a.type) == SwitchStateMachineState.TYPE;
    }
    getMarkerEnd() {
        return 'url(#' + (this.isDefault() ? 'statediagram-barbEndDefault' : 'statediagram-barbEnd') + ')';
    }
}
class EdgeLabel {
    constructor(label, edgePath) {
        this.label = label;
        this.edgePath = edgePath;
        this.posX = 0;
        this.posY = 0;
        this.width = 0;
        this.height = 0;
        this.selected = false;
    }
    computePosition() {
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
    constructor(diagramNode) {
        this.diagramNode = diagramNode;
        this.x = 0;
        this.y = 0;
        this.selected = false;
    }
    get transform() {
        return "translate(" + this.x + "," + this.y + ")";
    }
    computePosition(nodeWidth, nodeHeight) {
        this.x = this.diagramNode.x + (nodeWidth / 2);
        this.y = this.diagramNode.y + nodeHeight;
    }
}
class DraggableZone {
    constructor(diagramNode, anchor) {
        this.diagramNode = diagramNode;
        this.anchor = anchor;
        this.x = 0;
        this.y = 0;
        this.width = 0;
        this.height = 0;
    }
    computPosition(nodeWidth, nodeHeight, nodeGap, draggableZoneWidth) {
        this.x = this.diagramNode.x + (nodeWidth / 2) - (draggableZoneWidth / 2);
        this.y = this.diagramNode.y + (nodeHeight);
        this.width = draggableZoneWidth;
        this.height = nodeGap;
    }
}
class DiagramOptions {
    constructor() {
        this.canvasWidth = 800;
        this.canvasHeight = 600;
        this.nodeGap = 50;
        this.nodeWidth = 200;
        this.nodeHeight = 90;
        this.tokenWidth = 100;
        this.tokenHeight = 30;
        this.zoom = 20;
        this.draggableZoneWidth = 30;
        this.circleRadius = 7;
    }
    get titleHeight() {
        return this.nodeHeight * 0.30;
    }
    get bodyHeight() {
        return this.nodeHeight * 0.7;
    }
}
let StateDiagramComponent = class StateDiagramComponent {
    constructor(dialog) {
        this.dialog = dialog;
        this._stateMachine = new StateMachineModel();
        this._stateMachineInstance = new StateMachineInstanceDetails();
        this.options = new DiagramOptions();
        this.editMode = true;
        this.contextMenu = null;
        this.circleStartPosition = { x: 0, y: 0 };
        this.circleStartSelected = false;
        this.edgePathCircleStart = "";
        this.viewBox = "0 0 1 1";
        this.isMoving = false;
        this.isResizing = false;
        this.startMoving = false;
        this.previousMousePosition = { x: 0, y: 0 };
        this.nodes = [];
        this.edgePaths = [];
        this.edgeLabels = [];
        this.anchors = [];
        this.draggableZoneLst = [];
        this.tokens = [];
        this.selectedState = null;
        this.selectedTransition = null;
        this.handleMouseMoveWindowRef = null;
        this.handleMouseUpRef = null;
        this.gutterBoundingRect = null;
    }
    get stateMachine() {
        return this._stateMachine;
    }
    set stateMachine(s) {
        this._stateMachine = s;
        if (s) {
            this.refreshUI();
        }
    }
    get stateMachineInstance() {
        return this._stateMachineInstance;
    }
    set stateMachineInstance(s) {
        this._stateMachineInstance = s;
        if (s) {
            this.refreshUI();
        }
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
    onDragStart(evt, type) {
        evt.dataTransfer.setData('type', type);
    }
    onDrop(evt, draggableZone) {
        var _a, _b;
        if (this.startMoving) {
            return;
        }
        draggableZone.anchor.selected = false;
        const parent = draggableZone.diagramNode.state;
        let child = null;
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
            const removedTransition = parent === null || parent === void 0 ? void 0 : parent.tryAddTransition(child.id);
            if (removedTransition) {
                (_a = this.stateMachine) === null || _a === void 0 ? void 0 : _a.removeByNames([removedTransition]);
            }
            (_b = this.stateMachine) === null || _b === void 0 ? void 0 : _b.states.push(child);
            this.refreshUI();
            this.closePanel();
        }
    }
    onDragOver(evt, draggableZone) {
        if (this.startMoving) {
            return;
        }
        evt.preventDefault();
        draggableZone.anchor.selected = true;
    }
    onDragOverCircleStart(evt) {
        if (this.startMoving) {
            return;
        }
        this.circleStartSelected = true;
        evt.preventDefault();
    }
    onDragLeaveCircleStart(evt) {
        if (this.startMoving) {
            return;
        }
        this.circleStartSelected = false;
    }
    onDropCircleStart(evt) {
        var _a;
        if (this.startMoving) {
            this.circleStartSelected = false;
            return;
        }
        this.circleStartSelected = false;
        let child = null;
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
            (_a = this.stateMachine) === null || _a === void 0 ? void 0 : _a.states.push(child);
            this.refreshUI();
            this.closePanel();
        }
    }
    onDragLeave(evt, draggableZone) {
        if (this.startMoving) {
            return;
        }
        draggableZone.anchor.selected = false;
    }
    removeNode(node) {
        var _a, _b;
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
                (_a = this.stateMachine) === null || _a === void 0 ? void 0 : _a.remove(state);
                this.refreshUI();
                return;
            }
        }
        let parentNextTransitions = parent === null || parent === void 0 ? void 0 : parent.getNextTransitions();
        parentNextTransitions = parentNextTransitions === null || parentNextTransitions === void 0 ? void 0 : parentNextTransitions.filter((t) => t.transition !== (state === null || state === void 0 ? void 0 : state.id));
        (_b = this.stateMachine) === null || _b === void 0 ? void 0 : _b.remove(state);
        if (parentNextTransitions) {
            parent === null || parent === void 0 ? void 0 : parent.setTransitions(parentNextTransitions);
        }
        this.refreshUI();
        this.closePanel();
    }
    selectState(node) {
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
    selectLabel(label) {
        this.edgeLabels.forEach((e) => {
            e.selected = false;
        });
        this.selectedState = null;
        label.selected = !label.selected;
        this.selectedTransition = label.edgePath.transition;
    }
    onContextMenu(evt, edgeLabel, index) {
        var _a;
        evt.preventDefault();
        if (edgeLabel.edgePath.transition.getType() == "empty") {
            return;
        }
        const matMenuTrigger = (_a = this.contextMenu) === null || _a === void 0 ? void 0 : _a.get(index);
        matMenuTrigger === null || matMenuTrigger === void 0 ? void 0 : matMenuTrigger.openMenu();
    }
    displayToken(token) {
        this.dialog.open(TokenComponent, {
            data: token.data,
            width: '800px'
        });
    }
    setDefaultCondition(edgeLabel) {
        const switchStateMachine = edgeLabel.edgePath.formNode.state;
        switchStateMachine.swichTransitionToDefault(edgeLabel.edgePath.transition);
        this.refreshUI();
        this.selectedTransition = null;
    }
    initListener() {
        this.initStateDiagramListeners();
    }
    initStateDiagramListeners() {
        const self = this;
        const native = this.stateDiagram.nativeElement;
        const viewBox = native.viewBox;
        native.onmousedown = function (e) {
            if (!self.startMoving) {
                return;
            }
            self.isMoving = true;
            self.previousMousePosition = { x: e.clientX + viewBox.animVal.x, y: e.clientY + viewBox.animVal.y };
        };
        native.onmousemove = function (e) {
            if (!self.isMoving) {
                return;
            }
            const diffX = -(e.clientX - self.previousMousePosition.x);
            const diffY = -(e.clientY - self.previousMousePosition.y);
            self.viewBox = diffX + " " + diffY + " " + viewBox.animVal.width + " " + viewBox.animVal.height;
        };
        native.onmouseup = function (e) {
            self.isMoving = false;
        };
    }
    handleMouseMoveWindow(e) {
        if (!this.isResizing) {
            return;
        }
        const stateDiagramContainer = this.stateDiagramContainer.nativeElement;
        const newX = this.previousMousePosition.x - e.x;
        stateDiagramContainer.style.width = this.gutterBoundingRect.width - newX + "px";
    }
    handleMouseUp(e) {
        if (this.isResizing) {
            this.isResizing = false;
        }
        if (this.isMoving) {
            this.isMoving = false;
        }
    }
    zoom(delta) {
        const native = this.stateDiagram.nativeElement;
        const viewBox = native.viewBox;
        const w = viewBox.animVal.width;
        const h = viewBox.animVal.height;
        this.viewBox = (viewBox.animVal.x) + " " + (viewBox.animVal.y) + " " + (w + delta) + " " + (h + delta);
    }
    refreshUI() {
        var _a;
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
        const rootState = (_a = this.stateMachine) === null || _a === void 0 ? void 0 : _a.getRootState();
        if (!rootState) {
            return;
        }
        this.buildNodeHierarchy(rootState, undefined);
        this.updateNodePosition();
        this.updateStartAndEndEdgePath();
    }
    buildNodeHierarchy(state, parentNode) {
        const self = this;
        let newNode = new DiagramNode(0, 0, 0, state);
        if (parentNode) {
            newNode.level = parentNode.level + 1;
        }
        let existingNodes = this.nodes.filter((n) => { var _a; return ((_a = n.state) === null || _a === void 0 ? void 0 : _a.id) === state.id; });
        if (existingNodes.length === 1) {
            const existingNode = existingNodes[0];
            if (existingNode.level < newNode.level) {
                existingNode.level = newNode.level;
            }
            return existingNode;
        }
        if (self._stateMachineInstance && self._stateMachineInstance.states) {
            let filteredStateInstances = self._stateMachineInstance.states.filter((s) => {
                var _a;
                return s.defId === ((_a = newNode.state) === null || _a === void 0 ? void 0 : _a.id);
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
        nextNodeNames.forEach((transition) => {
            var _a;
            const child = (_a = this.stateMachine) === null || _a === void 0 ? void 0 : _a.states.filter((s) => s.id === transition.transition)[0];
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
    updateNodePosition() {
        const totalWidth = this.getSvgWidth();
        if (!totalWidth) {
            return;
        }
        const groupedResult = this.groupBy(this.nodes, 'level');
        for (var key in groupedResult) {
            let index = 0;
            const nbNodes = groupedResult[key].length;
            groupedResult[key].forEach((n) => {
                n.computePosition(index, nbNodes, this.options.nodeGap, this.options.nodeWidth, this.options.nodeHeight, totalWidth);
                index++;
            });
        }
        this.edgePaths.forEach((ep) => {
            ep.computePosition(this.options.nodeWidth, this.options.nodeHeight);
        });
        this.edgeLabels.forEach((el) => {
            el.computePosition();
        });
        this.anchors.forEach((a) => {
            a.computePosition(this.options.nodeWidth, this.options.nodeHeight);
        });
        this.draggableZoneLst.forEach((d) => {
            d.computPosition(this.options.nodeWidth, this.options.nodeHeight, this.options.nodeGap, this.options.draggableZoneWidth);
        });
        this.tokens.forEach((t) => {
            t.computePosition(this.options.tokenWidth, this.options.tokenHeight, this.options.nodeHeight);
        });
    }
    updateStartAndEndEdgePath() {
        if (this.nodes.length === 0) {
            return;
        }
        const formX = this.circleStartPosition.x;
        const formY = this.circleStartPosition.y;
        const toX = this.nodes[0].x + (this.options.nodeWidth / 2);
        const toY = this.nodes[0].y;
        this.edgePathCircleStart = "M" + formX + "," + formY + "L" + toX + "," + toY;
    }
    computeCirclePosition() {
        const totalWidth = this.getSvgWidth();
        if (!totalWidth) {
            return 0;
        }
        return (totalWidth / 2);
    }
    groupBy(xs, key) {
        return xs.reduce(function (rv, x) {
            (rv[x[key]] = rv[x[key]] || []).push(x);
            return rv;
        }, {});
    }
    getParent(state) {
        const filtered = this.edgePaths.filter((e) => {
            var _a;
            return ((_a = e.targetNode.state) === null || _a === void 0 ? void 0 : _a.id) == state.id;
        });
        return filtered.length === 1 ? filtered[0].formNode.state : null;
    }
    buildId() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }
    getSvgWidth() {
        if (!this.stateDiagram) {
            return null;
        }
        const native = this.stateDiagram.nativeElement;
        return native.width.baseVal.value;
    }
};
__decorate([
    Input()
], StateDiagramComponent.prototype, "stateMachine", null);
__decorate([
    Input()
], StateDiagramComponent.prototype, "stateMachineInstance", null);
__decorate([
    Input()
], StateDiagramComponent.prototype, "options", void 0);
__decorate([
    Input()
], StateDiagramComponent.prototype, "editMode", void 0);
__decorate([
    ViewChild("stateDiagram")
], StateDiagramComponent.prototype, "stateDiagram", void 0);
__decorate([
    ViewChild("gutter")
], StateDiagramComponent.prototype, "gutter", void 0);
__decorate([
    ViewChild("stateDiagramContainer")
], StateDiagramComponent.prototype, "stateDiagramContainer", void 0);
__decorate([
    ViewChildren(MatMenuTrigger)
], StateDiagramComponent.prototype, "contextMenu", void 0);
StateDiagramComponent = __decorate([
    Component({
        selector: 'state-diagram',
        templateUrl: './statediagram.component.html',
        styleUrls: ['./statediagram.component.scss']
    })
], StateDiagramComponent);
export { StateDiagramComponent };
//# sourceMappingURL=statediagram.component.js.map