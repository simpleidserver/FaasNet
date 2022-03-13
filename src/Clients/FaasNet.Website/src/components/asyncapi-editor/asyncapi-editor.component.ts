import { Component, Input, OnDestroy, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { MatMenuTrigger } from '@angular/material/menu';
import { MatPanelService } from '../matpanel/matpanelservice';
import { Application } from './models/application.model';
import { ApplicationLink } from './models/link.model';
import { ViewAsyncApiComponent, ViewAsyncApiData } from './viewasyncapicomponent';

class AsyncApiEditorOptions {
  zoom: number = 20;
  applicationWidth: number = 200;
  applicationHeight: number = 80;
}

class Anchor {
  constructor(public x: number, public y: number) { }

  get coordinate() {
    return `translate(${this.x},${this.y})`;
  }

  isSelected: boolean = false;
  isEditable: boolean = true;
}

class DraggableZone {
  constructor(public x: number, public y: number, public width: number, public height: number) { }
}

class Link {
  isSelected: boolean = false;
  startElement: Element | null = null;
  endElement: Element | null = null;
  applicationLink: ApplicationLink = new ApplicationLink();

  constructor(public id: string, public startAnchor: Anchor, public endAnchor: Anchor) { }

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
  constructor(public x: number, public y: number, public width: number, public height: number, public anchor: Anchor) {
    super(x, y, width, height);
  }
}

class Element {
  draggableZones: ElementDraggableZone[] = [];
  isSelected: boolean = false;
  dragOffset: { x: number, y: number } | null = null;

  constructor(public application: Application, public width: number, public height: number) {
    this.init();
  }

  public intersect(anchor: Anchor | undefined): Anchor | null {
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

  public update() {
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

  private init() {
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

@Component({
  selector: 'asyncapi-editor',
  templateUrl: './asyncapi-editor.component.html',
  styleUrls: ['./asyncapi-editor.component.scss']
})
export class AsyncApiEditorComponent implements OnInit, OnDestroy {
  startMoving: boolean = false;
  isMoving: boolean = false;
  viewBox: string = "0 0 1 1";
  selectedAnchorLink: {
    link: Link, anchor: Anchor, isStart: boolean, dragOffset: { x: number, y: number }
  } | null = null;
  selectedAnchorElement: { element: Element, anchor: Anchor } | null = null;
  selectedLink: Link | null = null;
  selectedElement: Element | null = null;
  previousPoint: any | null = null;
  previousMousePosition: { x: number, y: number } = { x: 0, y: 0 };
  @Input() editMode: boolean = true;
  @Input() options: AsyncApiEditorOptions = new AsyncApiEditorOptions();
  @ViewChild("stateDiagram") stateDiagram: any;
  @ViewChild("gutter") gutter: any;
  @ViewChild("stateDiagramContainer") stateDiagramContainer: any;
  @ViewChildren(MatMenuTrigger) contextMenu: QueryList<MatMenuTrigger> | null = null;
  elements: Element[] = [];
  links: Link[] = [];

  constructor(private matPanelService: MatPanelService) { }

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

  onDragStart(evt: any, id: string) {
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

  removeElement(i: number) {
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
    const consumedLinks = this.links.filter(l => l.endElement && l.endElement.application.id === this.selectedElement?.application.id).map(l => l.applicationLink);
    data.application = this.selectedElement.application;
    data.consumedLinks = consumedLinks;
    this.matPanelService.open(ViewAsyncApiComponent, data);
  }

  onDrop(evt: any) {
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

  onDragOver(evt: any) {
    evt.preventDefault();
  }

  onDragLeave(evt: any) {

  }

  private initListeners() {
    const self = this;
    const native = this.stateDiagram.nativeElement;
    const viewBox = native.viewBox;
    native.onmousedown = function (e: any) {
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
    native.onmousemove = function (e: any) {
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
    native.onmouseup = function (e: any) {
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

  private zoom(delta: number) {
    const native = this.stateDiagram.nativeElement;
    const viewBox = native.viewBox;
    const w = viewBox.animVal.width;
    const h = viewBox.animVal.height;
    this.viewBox = (viewBox.animVal.x) + " " + (viewBox.animVal.y) + " " + (w + delta) + " " + (h + delta);
  }

  private selectLink(e: any) {
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

  private selectElement(e: any) {
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

  private startMovingAnchor(e: any) {
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

  private moveSelectedAnchorLink(e: any, cursor: any) {
    const self = this;
    if (!self.selectedAnchorLink || !self.selectedAnchorLink.anchor) {
      return;
    }

    self.selectedAnchorLink.anchor.x = cursor.x - self.selectedAnchorLink?.dragOffset.x;
    self.selectedAnchorLink.anchor.y = cursor.y - self.selectedAnchorLink?.dragOffset.y;
    const filteredElts = self.elements.filter(e => e.intersect(self.selectedAnchorLink?.anchor));
    if (filteredElts.length === 1) {
      const selectedElement = filteredElts[0];
      const anchor = selectedElement.intersect(self.selectedAnchorLink?.anchor);
      if (anchor !== null) {
        anchor.isSelected = true;
        self.selectedAnchorElement = { element: selectedElement, anchor: anchor };
      }
    } else if (self.selectedAnchorElement) {
      self.selectedAnchorElement.anchor.isSelected = false;
      self.selectedAnchorElement = null;
    }
  }

  private moveSelectedApplication(e: any, cursor: any) {
    const self = this;
    if (!self.selectedElement || !self.selectedElement.dragOffset) {
      return;
    }

    self.selectedElement.application.posX = cursor.x - self.selectedElement.dragOffset.x;
    self.selectedElement.application.posY = cursor.y - self.selectedElement.dragOffset.y;
    self.selectedElement.update();
  }

  private connectLinkAnchor() {
    const self = this;
    if (!self.selectedAnchorLink || !self.selectedAnchorElement) {
      return;
    }

    if (self.selectedAnchorLink.isStart) {
      self.selectedAnchorLink.link.startAnchor = self.selectedAnchorElement.anchor;
      self.selectedAnchorLink.link.startAnchor.isEditable = false;
      self.selectedAnchorLink.link.startElement = self.selectedAnchorElement.element;
    } else {
      self.selectedAnchorLink.link.endAnchor = self.selectedAnchorElement.anchor;
      self.selectedAnchorLink.link.endAnchor.isEditable = false;
      self.selectedAnchorLink.link.endElement = self.selectedAnchorElement.element;
      self.selectedAnchorLink.link.applicationLink.target = self.selectedAnchorElement.element.application;
    }

    if (self.selectedAnchorLink.link.isClosed) {
      self.selectedAnchorLink.link.startElement?.application.links.push(self.selectedAnchorLink.link.applicationLink);
    }
  }

  private newGUID() {
    var u = '', i = 0;
    while (i++ < 36) {
      var c = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'[i - 1], r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
      u += (c == '-' || c == '4') ? c : v.toString(16)
    }
    return u;
  }

  private getCoordinate(evt: any): any {
    const native = this.stateDiagram.nativeElement;
    let point = native.createSVGPoint();
    point = this.updateCoordinate(evt, point);
    return point;
  }

  private updateCoordinate(evt: any, point: any) {
    const native = this.stateDiagram.nativeElement;
    point.x = evt.clientX;
    point.y = evt.clientY;
    return point.matrixTransform(native.getScreenCTM().inverse());
  }

  private getAnchorLink(e: any) {
    let targetElement = e.target;
    if (!targetElement.classList.contains('anchor')) {
      if (!targetElement.parentElement || !targetElement.parentElement.classList.contains('anchor')) {
        return null;
      }

      targetElement = targetElement.parentElement;
    }

    return targetElement;
  }

  private getElement(e: any) {
    let targetElement = e.target;
    if (!targetElement.classList.contains('content')) {
      if (!targetElement.parentElement || !targetElement.parentElement.classList.contains('content')) {
        return null;
      }

      targetElement = targetElement.parentElement;
    }

    return targetElement;
  }
}
