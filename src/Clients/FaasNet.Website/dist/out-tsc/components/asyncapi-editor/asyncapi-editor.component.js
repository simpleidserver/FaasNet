import { __decorate } from "tslib";
import { Component, Input, ViewChild, ViewChildren } from '@angular/core';
import { MatMenuTrigger } from '@angular/material/menu';
import { Application } from './models/application.model';
import { ApplicationLink } from './models/link.model';
import { ViewAsyncApiComponent, ViewAsyncApiData } from './viewasyncapicomponent';
class AsyncApiEditorOptions {
    constructor() {
        this.zoom = 20;
        this.applicationWidth = 200;
        this.applicationHeight = 80;
    }
}
class Anchor {
    constructor(x, y) {
        this.x = x;
        this.y = y;
        this.isSelected = false;
        this.isEditable = true;
    }
    get coordinate() {
        return `translate(${this.x},${this.y})`;
    }
}
class DraggableZone {
    constructor(x, y, width, height) {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }
}
class Link {
    constructor(id, startAnchor, endAnchor) {
        this.id = id;
        this.startAnchor = startAnchor;
        this.endAnchor = endAnchor;
        this.isSelected = false;
        this.startElement = null;
        this.endElement = null;
        this.applicationLink = new ApplicationLink();
    }
    get path() {
        return `M${this.startAnchor.x},${this.startAnchor.y},L${this.endAnchor.x},${this.endAnchor.y}`;
    }
    get startAnchorCoordinate() {
        return `translate(${this.startAnchor.x},${this.startAnchor.y})`;
    }
    get endAnchorCoordinate() {
        return `translate(${this.endAnchor.x},${this.endAnchor.y})`;
    }
    get selectionZoneX() {
        return this.startAnchor.x < this.endAnchor.x ? this.startAnchor.x : this.endAnchor.x;
    }
    get selectionZoneY() {
        return this.startAnchor.y < this.endAnchor.y ? this.startAnchor.y : this.endAnchor.y;
    }
    get selectionZoneWidth() {
        var result = this.startAnchor.x - this.endAnchor.x;
        result = result < 0 ? -result : result;
        return result;
    }
    get selectionZoneHeight() {
        var result = this.startAnchor.y - this.endAnchor.y;
        result = result < 0 ? -result : result;
        return result;
    }
    get isClosed() {
        return this.startElement && this.endElement;
    }
}
class ElementDraggableZone extends DraggableZone {
    constructor(x, y, width, height, anchor) {
        super(x, y, width, height);
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.anchor = anchor;
    }
}
class Element {
    constructor(application, width, height) {
        this.application = application;
        this.width = width;
        this.height = height;
        this.draggableZones = [];
        this.isSelected = false;
        this.dragOffset = null;
        this.init();
    }
    intersect(anchor) {
        if (!anchor) {
            return null;
        }
        const filteredDraggableZones = this.draggableZones.filter(z => anchor.x >= z.x && anchor.x <= (z.x + z.width) &&
            anchor.y >= z.y && anchor.y <= (z.y + z.height));
        if (filteredDraggableZones.length !== 1) {
            return null;
        }
        const selectedDraggableZone = filteredDraggableZones[0];
        return selectedDraggableZone.anchor;
    }
    update() {
        const draggableZoneWidth = 50;
        const draggableZoneHeight = 50;
        let draggableZoneWidthMiddle = draggableZoneWidth / 2;
        let draggableZoneHeightMiddle = draggableZoneHeight / 2;
        const leftAnchor = new Anchor(this.application.posX, this.application.posY + (this.height / 2));
        const topAnchor = new Anchor(this.application.posX + (this.width / 2), this.application.posY);
        const rightAnchor = new Anchor(this.application.posX + this.width, this.application.posY + (this.height / 2));
        const bottomAnchor = new Anchor(this.application.posX + (this.width / 2), this.application.posY + this.height);
        this.draggableZones[0].anchor.x = leftAnchor.x;
        this.draggableZones[0].anchor.y = leftAnchor.y;
        this.draggableZones[0].x = leftAnchor.x - draggableZoneWidthMiddle;
        this.draggableZones[0].y = leftAnchor.y - draggableZoneHeightMiddle;
        this.draggableZones[1].anchor.x = topAnchor.x;
        this.draggableZones[1].anchor.y = topAnchor.y;
        this.draggableZones[1].x = topAnchor.x - draggableZoneWidthMiddle;
        this.draggableZones[1].y = topAnchor.y - draggableZoneHeightMiddle;
        this.draggableZones[2].anchor.x = rightAnchor.x;
        this.draggableZones[2].anchor.y = rightAnchor.y;
        this.draggableZones[2].x = rightAnchor.x - draggableZoneWidthMiddle;
        this.draggableZones[2].y = rightAnchor.y - draggableZoneHeightMiddle;
        this.draggableZones[3].anchor.x = bottomAnchor.x;
        this.draggableZones[3].anchor.y = bottomAnchor.y;
        this.draggableZones[3].x = bottomAnchor.x - draggableZoneWidthMiddle;
        this.draggableZones[3].y = bottomAnchor.y - draggableZoneHeightMiddle;
    }
    init() {
        const draggableZoneWidth = 50;
        const draggableZoneHeight = 50;
        let draggableZoneWidthMiddle = draggableZoneWidth / 2;
        let draggableZoneHeightMiddle = draggableZoneHeight / 2;
        const leftAnchor = new Anchor(this.application.posX, this.application.posY + (this.height / 2));
        const topAnchor = new Anchor(this.application.posX + (this.width / 2), this.application.posY);
        const rightAnchor = new Anchor(this.application.posX + this.width, this.application.posY + (this.height / 2));
        const bottomAnchor = new Anchor(this.application.posX + (this.width / 2), this.application.posY + this.height);
        const leftDraggableZone = new ElementDraggableZone(leftAnchor.x - draggableZoneWidthMiddle, leftAnchor.y - draggableZoneHeightMiddle, draggableZoneWidth, draggableZoneHeight, leftAnchor);
        const topDraggableZone = new ElementDraggableZone(topAnchor.x - draggableZoneWidthMiddle, topAnchor.y - draggableZoneHeightMiddle, draggableZoneWidth, draggableZoneHeight, topAnchor);
        const rightDraggableZone = new ElementDraggableZone(rightAnchor.x - draggableZoneWidthMiddle, rightAnchor.y - draggableZoneHeightMiddle, draggableZoneWidth, draggableZoneHeight, rightAnchor);
        const bottomDraggableZone = new ElementDraggableZone(bottomAnchor.x - draggableZoneWidthMiddle, bottomAnchor.y - draggableZoneHeightMiddle, draggableZoneWidth, draggableZoneHeight, bottomAnchor);
        this.draggableZones.push(leftDraggableZone);
        this.draggableZones.push(topDraggableZone);
        this.draggableZones.push(rightDraggableZone);
        this.draggableZones.push(bottomDraggableZone);
    }
}
let AsyncApiEditorComponent = class AsyncApiEditorComponent {
    constructor(matPanelService) {
        this.matPanelService = matPanelService;
        this.startMoving = false;
        this.isMoving = false;
        this.viewBox = "0 0 1 1";
        this.selectedAnchorLink = null;
        this.selectedAnchorElement = null;
        this.selectedLink = null;
        this.selectedElement = null;
        this.previousPoint = null;
        this.previousMousePosition = { x: 0, y: 0 };
        this.editMode = true;
        this.options = new AsyncApiEditorOptions();
        this.contextMenu = null;
        this.elements = [];
        this.links = [];
    }
    get isSelected() {
        return this.selectedLink !== null || this.selectedElement !== null;
    }
    ngOnInit() {
    }
    ngOnDestroy() {
    }
    ngAfterViewInit() {
        this.viewBox = "0 0 " + this.stateDiagramContainer.nativeElement.offsetWidth + " " + this.stateDiagramContainer.nativeElement.offsetHeight;
        this.initListeners();
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
    onDragStart(evt, id) {
        evt.dataTransfer.setData('type', id);
    }
    unselectElement() {
        if (this.selectedElement) {
            this.selectedElement.isSelected = false;
        }
        this.selectedElement = null;
    }
    unselectLink() {
        if (this.selectedLink) {
            this.selectedLink.isSelected = false;
        }
        this.selectedLink = null;
    }
    removeElement(i) {
        const elt = this.elements[i];
        this.links.filter(l => l.startElement && l.startElement.application.id === elt.application.id).forEach((e) => e.startAnchor.isEditable = true);
        this.links.filter(l => l.endElement && l.endElement.application.id === elt.application.id).forEach((e) => e.endAnchor.isEditable = true);
        this.elements.splice(i, 1);
    }
    remove() {
        if (this.selectedLink) {
            const linkIndex = this.links.indexOf(this.selectedLink);
            this.links.splice(linkIndex, 1);
            return;
        }
        if (this.selectedElement) {
            const eltIndex = this.elements.indexOf(this.selectedElement);
            this.elements.splice(eltIndex, 1);
            return;
        }
    }
    viewAsyncApi() {
        if (!this.selectedElement) {
            return;
        }
        const data = new ViewAsyncApiData();
        const consumedLinks = this.links.filter(l => { var _a; return l.endElement && l.endElement.application.id === ((_a = this.selectedElement) === null || _a === void 0 ? void 0 : _a.application.id); }).map(l => l.applicationLink);
        data.application = this.selectedElement.application;
        data.consumedLinks = consumedLinks;
        this.matPanelService.open(ViewAsyncApiComponent, data);
    }
    onDrop(evt) {
        const point = this.getCoordinate(evt);
        const type = evt.dataTransfer.getData('type');
        switch (type) {
            case 'application':
                const application = new Application();
                application.id = this.newGUID();
                application.title = "app";
                application.posX = point.x;
                application.posY = point.y;
                this.elements.push(new Element(application, this.options.applicationWidth, this.options.applicationHeight));
                break;
            case 'link':
                const startAnchor = new Anchor(point.x, point.y);
                const endAnchor = new Anchor(point.x + 10, point.y + 100);
                const link = new Link(this.newGUID(), startAnchor, endAnchor);
                this.links.push(link);
                break;
        }
    }
    onDragOver(evt) {
        evt.preventDefault();
    }
    onDragLeave(evt) {
    }
    initListeners() {
        const self = this;
        const native = this.stateDiagram.nativeElement;
        const viewBox = native.viewBox;
        native.onmousedown = function (e) {
            self.isMoving = true;
            self.previousPoint = self.getCoordinate(e);
            self.elements.forEach((e) => e.isSelected = false);
            self.links.forEach((e) => e.isSelected = false);
            if (!self.startMoving) {
                self.selectLink(e);
                self.startMovingAnchor(e);
                self.selectElement(e);
                return;
            }
            self.previousMousePosition = { x: e.clientX + viewBox.animVal.x, y: e.clientY + viewBox.animVal.y };
        };
        native.onmousemove = function (e) {
            if (!self.isMoving) {
                return;
            }
            if (!self.startMoving) {
                let cursor = self.updateCoordinate(e, self.previousPoint);
                self.moveSelectedAnchorLink(e, cursor);
                self.moveSelectedApplication(e, cursor);
                return;
            }
            const diffX = -(e.clientX - self.previousMousePosition.x);
            const diffY = -(e.clientY - self.previousMousePosition.y);
            self.viewBox = diffX + " " + diffY + " " + viewBox.animVal.width + " " + viewBox.animVal.height;
        };
        native.onmouseup = function (e) {
            if (!self.isMoving) {
                return;
            }
            self.connectLinkAnchor();
            self.isMoving = false;
            self.selectedAnchorLink = null;
            if (self.selectedAnchorElement) {
                self.selectedAnchorElement.anchor.isSelected = false;
            }
            self.selectedAnchorElement = null;
        };
    }
    zoom(delta) {
        const native = this.stateDiagram.nativeElement;
        const viewBox = native.viewBox;
        const w = viewBox.animVal.width;
        const h = viewBox.animVal.height;
        this.viewBox = (viewBox.animVal.x) + " " + (viewBox.animVal.y) + " " + (w + delta) + " " + (h + delta);
    }
    selectLink(e) {
        if (this.selectedLink) {
            this.selectedLink.isSelected = false;
        }
        const targetElement = e.target;
        const linkAnchor = this.getAnchorLink(e);
        if (!targetElement.classList.contains('linkSelection') && !linkAnchor) {
            this.selectedLink = null;
            return;
        }
        let id = targetElement.getAttribute('linkId');
        if (!id) {
            id = linkAnchor.getAttribute('linkId');
        }
        const link = this.links.filter(l => l.id === id)[0];
        this.selectedLink = link;
        this.selectedLink.isSelected = true;
    }
    selectElement(e) {
        if (this.selectedElement) {
            this.selectedElement.isSelected = false;
        }
        const targetElement = this.getElement(e);
        if (!targetElement) {
            this.selectedElement = null;
            return;
        }
        const id = targetElement.getAttribute('elementId');
        const element = this.elements.filter(e => e.application.id === id)[0];
        this.selectedElement = element;
        this.selectedElement.dragOffset = { x: this.previousPoint.x - element.application.posX, y: this.previousPoint.y - element.application.posY };
        this.selectedElement.isSelected = true;
    }
    startMovingAnchor(e) {
        let targetElement = this.getAnchorLink(e);
        if (!targetElement) {
            return;
        }
        const isStart = targetElement.classList.contains('start');
        const id = targetElement.getAttribute('linkId');
        const link = this.links.filter((l) => l.id === id)[0];
        const anchor = isStart ? link.startAnchor : link.endAnchor;
        this.selectedAnchorLink = {
            anchor: anchor, link: link, isStart: isStart, dragOffset: { x: this.previousPoint.x - anchor.x, y: this.previousPoint.y - anchor.y }
        };
    }
    moveSelectedAnchorLink(e, cursor) {
        var _a, _b, _c;
        const self = this;
        if (!self.selectedAnchorLink || !self.selectedAnchorLink.anchor) {
            return;
        }
        self.selectedAnchorLink.anchor.x = cursor.x - ((_a = self.selectedAnchorLink) === null || _a === void 0 ? void 0 : _a.dragOffset.x);
        self.selectedAnchorLink.anchor.y = cursor.y - ((_b = self.selectedAnchorLink) === null || _b === void 0 ? void 0 : _b.dragOffset.y);
        const filteredElts = self.elements.filter(e => { var _a; return e.intersect((_a = self.selectedAnchorLink) === null || _a === void 0 ? void 0 : _a.anchor); });
        if (filteredElts.length === 1) {
            const selectedElement = filteredElts[0];
            const anchor = selectedElement.intersect((_c = self.selectedAnchorLink) === null || _c === void 0 ? void 0 : _c.anchor);
            if (anchor !== null) {
                anchor.isSelected = true;
                self.selectedAnchorElement = { element: selectedElement, anchor: anchor };
            }
        }
        else if (self.selectedAnchorElement) {
            self.selectedAnchorElement.anchor.isSelected = false;
            self.selectedAnchorElement = null;
        }
    }
    moveSelectedApplication(e, cursor) {
        const self = this;
        if (!self.selectedElement || !self.selectedElement.dragOffset) {
            return;
        }
        self.selectedElement.application.posX = cursor.x - self.selectedElement.dragOffset.x;
        self.selectedElement.application.posY = cursor.y - self.selectedElement.dragOffset.y;
        self.selectedElement.update();
    }
    connectLinkAnchor() {
        var _a;
        const self = this;
        if (!self.selectedAnchorLink || !self.selectedAnchorElement) {
            return;
        }
        if (self.selectedAnchorLink.isStart) {
            self.selectedAnchorLink.link.startAnchor = self.selectedAnchorElement.anchor;
            self.selectedAnchorLink.link.startAnchor.isEditable = false;
            self.selectedAnchorLink.link.startElement = self.selectedAnchorElement.element;
        }
        else {
            self.selectedAnchorLink.link.endAnchor = self.selectedAnchorElement.anchor;
            self.selectedAnchorLink.link.endAnchor.isEditable = false;
            self.selectedAnchorLink.link.endElement = self.selectedAnchorElement.element;
            self.selectedAnchorLink.link.applicationLink.target = self.selectedAnchorElement.element.application;
        }
        if (self.selectedAnchorLink.link.isClosed) {
            (_a = self.selectedAnchorLink.link.startElement) === null || _a === void 0 ? void 0 : _a.application.links.push(self.selectedAnchorLink.link.applicationLink);
        }
    }
    newGUID() {
        var u = '', i = 0;
        while (i++ < 36) {
            var c = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'[i - 1], r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            u += (c == '-' || c == '4') ? c : v.toString(16);
        }
        return u;
    }
    getCoordinate(evt) {
        const native = this.stateDiagram.nativeElement;
        let point = native.createSVGPoint();
        point = this.updateCoordinate(evt, point);
        return point;
    }
    updateCoordinate(evt, point) {
        const native = this.stateDiagram.nativeElement;
        point.x = evt.clientX;
        point.y = evt.clientY;
        return point.matrixTransform(native.getScreenCTM().inverse());
    }
    getAnchorLink(e) {
        let targetElement = e.target;
        if (!targetElement.classList.contains('anchor')) {
            if (!targetElement.parentElement || !targetElement.parentElement.classList.contains('anchor')) {
                return null;
            }
            targetElement = targetElement.parentElement;
        }
        return targetElement;
    }
    getElement(e) {
        let targetElement = e.target;
        if (!targetElement.classList.contains('content')) {
            if (!targetElement.parentElement || !targetElement.parentElement.classList.contains('content')) {
                return null;
            }
            targetElement = targetElement.parentElement;
        }
        return targetElement;
    }
};
__decorate([
    Input()
], AsyncApiEditorComponent.prototype, "editMode", void 0);
__decorate([
    Input()
], AsyncApiEditorComponent.prototype, "options", void 0);
__decorate([
    ViewChild("stateDiagram")
], AsyncApiEditorComponent.prototype, "stateDiagram", void 0);
__decorate([
    ViewChild("gutter")
], AsyncApiEditorComponent.prototype, "gutter", void 0);
__decorate([
    ViewChild("stateDiagramContainer")
], AsyncApiEditorComponent.prototype, "stateDiagramContainer", void 0);
__decorate([
    ViewChildren(MatMenuTrigger)
], AsyncApiEditorComponent.prototype, "contextMenu", void 0);
AsyncApiEditorComponent = __decorate([
    Component({
        selector: 'asyncapi-editor',
        templateUrl: './asyncapi-editor.component.html',
        styleUrls: ['./asyncapi-editor.component.scss']
    })
], AsyncApiEditorComponent);
export { AsyncApiEditorComponent };
//# sourceMappingURL=asyncapi-editor.component.js.map