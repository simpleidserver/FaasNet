import { Component, EventEmitter, Input, OnDestroy, Output } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { MatPanelService } from "../../../matpanel/matpanelservice";
import { ApplicationLink } from "../../models/link.model";
import { LinkEventsEditorComponent, LinkEvtsEditorData } from "./evtseditor.component";

@Component({
  selector: 'link-editor',
  templateUrl: './link-editor.component.html',
  styleUrls: [
    './link-editor.component.scss',
    '../editor.component.scss'
  ]
})
export class LinkEditorComponent implements OnDestroy {
  private _link: ApplicationLink | undefined = undefined;
  updateLinkFormGroup: FormGroup = new FormGroup({
  });
  @Input()
  get link(): ApplicationLink | undefined {
    return this._link;
  }
  set link(v: ApplicationLink | undefined) {
    this._link = v;
  }
  @Output() closed: EventEmitter<any> = new EventEmitter<any>();

  constructor(private matPanelService : MatPanelService) { }

  ngOnDestroy() {
  }

  close() {
    this.closed.emit();
  }

  getMessages() {
    return this._link?.evts.map(l => l.name).join(',');
  }

  editMessages() {
    const data = new LinkEvtsEditorData();
    if (this._link) {
      data.evts = this._link.evts;
    }

    this.matPanelService.open(LinkEventsEditorComponent, data);
  }
}
