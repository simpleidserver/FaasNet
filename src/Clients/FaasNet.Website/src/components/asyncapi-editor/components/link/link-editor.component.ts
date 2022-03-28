import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from "@angular/core";
import { FormControl } from "@angular/forms";
import { ApplicationLinkResult } from "@stores/applicationdomains/models/applicationlink.model";
import { MatPanelService } from "../../../matpanel/matpanelservice";
import { ChooseMessageComponent, ChooseMessageData } from "./choosemessage.component";
import * as fromReducers from '@stores/appstate';
import { getLatestMessages } from '@stores/messagedefinitions/actions/messagedefs.actions';
import { MessageDefinitionResult } from '@stores/messagedefinitions/models/messagedefinition.model';
import { select, Store } from '@ngrx/store';

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
  _message: MessageDefinitionResult | null = null;
  _messageDefs: MessageDefinitionResult[] = [];
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
      this.refresh();
    }
  }
  @Output() closed: EventEmitter<any> = new EventEmitter<any>();

  constructor(private matPanelService: MatPanelService, private store: Store<fromReducers.AppState>) { }

  ngOnInit() {
    this.store.pipe(select(fromReducers.selectMessageDefsResult)).subscribe((state: MessageDefinitionResult[] | null) => {
      if (!state) {
        return;
      }

      this._messageDefs = state;
      this._message = this._messageDefs.filter(m => m.id === this._link?.messageId)[0];
    });
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
    if (!this._message) {
      return '';
    }

    return this._message.name;
  }

  editMessage() {
    const data = new ChooseMessageData();
    if (this._link) {
      data.appDomainId = this.appDomainId;
      data.vpnName = this.vpnName;
      data.messageId = this._link.messageId;
    }

    const service = this.matPanelService.open(ChooseMessageComponent, data);
    service.closed.subscribe((e) => {
      if (!e || !this.link) {
        return;
      }

      if (!e.id) {
        this.link.messageId = null;
      } else {
        this.link.messageId = e.id;
        this._message = this._messageDefs.filter(m => m.id === this._link?.messageId)[0];
      }
    });
  }

  refresh() {
    const act = getLatestMessages({ appDomainId: this.appDomainId });
    this.store.dispatch(act);
  }
}
