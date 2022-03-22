import { Component, EventEmitter, Input, OnDestroy, Output } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { MatPanelService } from "../../../matpanel/matpanelservice";
import { Application } from "../../models/application.model";
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
  private _application: Application | undefined = undefined;
  private _titleSubscription: any;
  private _descriptionSubscription: any;
  updateApplicationFormGroup: FormGroup = new FormGroup({
    description: new FormControl(),
    version: new FormControl()
  });

  @Input() vpnName: string = "";
  @Input() appDomainId: string = "";
  @Input()
  get element() : Application | undefined {
    return this._application;
  }
  set element(v: Application | undefined) {
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
    return this._application?.title;
  }

  chooseClient() {
    const data = new ChooseClientData();
    if (this._application) {
      data.appDomainId = this.appDomainId;
      data.vpnName = this.vpnName;
    }

    const service = this.matPanelService.open(ChooseClientComponent, data);
    service.closed.subscribe((e) => {
      if (!e || !this._application) {
        return;
      }

      if (!e.clientId) {
        this._application.title = null;
      } else {
        this._application.title = e.clientId;
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
