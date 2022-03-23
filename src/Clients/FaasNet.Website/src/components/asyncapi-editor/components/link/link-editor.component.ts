import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from "@angular/core";
import { FormControl } from "@angular/forms";
import { ApplicationLinkResult } from "../../../../stores/vpn/models/applicationlink.model";
import { MatPanelService } from "../../../matpanel/matpanelservice";
import { ChooseMessageComponent, ChooseMessageData } from "./choosemessage.component";

@Component({
  selector: 'link-editor',
  templateUrl: './link-editor.component.html',
  styleUrls: [
    './link-editor.component.scss',
    '../editor.component.scss'
  ]
})
export class LinkEditorComponent implements OnInit, OnDestroy {
  private _link: ApplicationLinkResult | undefined = undefined;
  topicFormControl: FormControl = new FormControl();

  @Input() vpnName: string = "";
  @Input() appDomainId: string = "";
  @Input()
  get link(): ApplicationLinkResult | undefined {
    return this._link;
  }
  set link(v: ApplicationLinkResult | undefined) {
    this._link = v;
    if (this._link) {
      this.topicFormControl.setValue(this._link.topicName);
    }
  }
  @Output() closed: EventEmitter<any> = new EventEmitter<any>();

  constructor(private matPanelService: MatPanelService) { }

  ngOnInit() {
    this.topicFormControl.valueChanges.subscribe((e) => {
      if (this._link) {
        this._link.topicName = e;
      }
    });
  }

  ngOnDestroy() {
  }

  close() {
    this.closed.emit();
  }

  getMessage() {
    if (!this.link || !this.link.message) {
      return '';
    }

    return this._link?.message?.name;
  }
  editMessage() {
    const data = new ChooseMessageData();
    if (this._link) {
      data.appDomainId = this.appDomainId;
      data.vpnName = this.vpnName;
      if (this._link.message) {
        data.messageId = this._link.message.id;
      }
    }

    const service = this.matPanelService.open(ChooseMessageComponent, data);
    service.closed.subscribe((e) => {
      if (!e || !this.link) {
        return;
      }

      if (!e.id) {
        this.link.message = null;
      } else {
        this.link.message = e;
      }
    });
  }
}
