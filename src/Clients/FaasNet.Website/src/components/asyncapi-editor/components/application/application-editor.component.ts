import { Component, EventEmitter, Input, OnDestroy, Output } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { ApplicationResult } from "../../../../stores/vpn/models/application.model";
import { MatPanelService } from "../../../matpanel/matpanelservice";
import { ChooseClientComponent, ChooseClientData } from "./chooseclient.component";

@Component({
  selector: 'application-editor',
  templateUrl: './application-editor.component.html',
  styleUrls: [
    './application-editor.component.scss',
    '../editor.component.scss'
  ]
})
export class ApplicationEditorComponent implements OnDestroy {
  private _application: ApplicationResult | undefined = undefined;
  private _titleSubscription: any;
  private _descriptionSubscription: any;
  updateApplicationFormGroup: FormGroup = new FormGroup({
    title: new FormControl(),
    description: new FormControl(),
    version: new FormControl()
  });

  @Input() vpnName: string = "";
  @Input() appDomainId: string = "";
  @Input()
  get element() : ApplicationResult | undefined {
    return this._application;
  }
  set element(v: ApplicationResult | undefined) {
    this._application = v;
    this.init();
  }
  @Output() closed: EventEmitter<any> = new EventEmitter<any>();

  constructor(private matPanelService: MatPanelService) { }

  ngOnDestroy() {
    if (this._titleSubscription) {
      this._titleSubscription.unsubscribe();
    }

    if (this._descriptionSubscription) {
      this._descriptionSubscription.unsubscribe();
    }
  }

  getClient() {
    return this._application?.clientId;
  }

  chooseClient() {
    const data = new ChooseClientData();
    if (this._application) {
      data.appDomainId = this.appDomainId;
      data.vpnName = this.vpnName;
      data.clientId = this._application.clientId;
    }

    const service = this.matPanelService.open(ChooseClientComponent, data);
    service.closed.subscribe((e) => {
      if (!e || !this._application) {
        return;
      }

      if (!e.clientId) {
        this._application.clientId = null;
      } else {
        this._application.clientId = e.clientId;
      }
    });
  }

  close() {
    this.closed.emit();
  }

  private init() {
    const self = this;
    if (!self._application) {
      return;
    }

    self._titleSubscription = this.updateApplicationFormGroup.get('title')?.valueChanges.subscribe((e: any) => {
      if (self._application) {
        self._application.title = e;
      }
    });
    self._descriptionSubscription = this.updateApplicationFormGroup.get('description')?.valueChanges.subscribe((e: any) => {
      if (self._application) {
        self._application.description = e;
      }
    });
    this.updateApplicationFormGroup.get('title')?.setValue(self._application.title);
    this.updateApplicationFormGroup.get('description')?.setValue(self._application.description);
    this.updateApplicationFormGroup.get('version')?.setValue(self._application.version);
  }
}
