import { Component, EventEmitter, Input, Output } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { StateMachineEvent } from "@stores/statemachines/models/statemachine-event.model";
import { BaseTransition } from "@stores/statemachines/models/statemachine-state.model";
import { EventCondition } from "@stores/statemachines/models/statemachine-switch-state.model";
import { MatPanelService } from "../../../matpanel/matpanelservice";
import { ChooseEvtComponent, ChooseEvtData } from "./chooseevt.component";

@Component({
  selector: 'evtcondition-editor',
  templateUrl: './evtcondition.component.html',
  styleUrls: [
    './evtcondition.component.scss',
    '../state-editor.component.scss'
  ]
})
export class EvtConditionComponent {
  private eventRefSubscription: any | null = null;
  private _evtCondition: EventCondition | null = null;
  private _transition: BaseTransition | null = null;
  private _events: StateMachineEvent[] = [];
  @Output() closed: EventEmitter<any> = new EventEmitter<any>();
  @Input()
  get transition(): BaseTransition | null {
    return this._transition;
  }
  set transition(v: BaseTransition | null) {
    this._transition = v;
    this._evtCondition = v as EventCondition;
    this.init();
  }

  @Input()
  get events(): StateMachineEvent[] {
    return this._events;
  }
  set events(v: StateMachineEvent[]) {
    this._events = v;
  }

  updateEvtConditionFormGroup: FormGroup = new FormGroup({
    eventRef: new FormControl()
  });

  constructor(private matPanelService: MatPanelService) {
  }

  ngOnInit() { }

  ngOnDestroy() {
    if (this.eventRefSubscription) {
      this.eventRefSubscription.unsubscribe();
    }
  }

  close() {
    this.closed.emit();
  }

  editEvtRef() {
    const data = new ChooseEvtData();
    data.events = this._events;
    if (this._evtCondition) {
      data.evtCondition = this._evtCondition;
    }

    const service = this.matPanelService.open(ChooseEvtComponent, data);
    service.closed.subscribe((e) => {
      if (!e || !this._evtCondition) {
        return;
      }

      this._evtCondition.eventRef = e.eventRef;
      this._evtCondition.eventDataFilter = e.eventDataFilter;
    });
  }

  getEvtRef() {
    return this._evtCondition?.eventRef;
  }

  private init() {
    const self = this;
    this.ngOnDestroy();
    this.updateEvtConditionFormGroup.get('eventRef')?.setValue(this._evtCondition?.eventRef);
    this.eventRefSubscription = this.updateEvtConditionFormGroup.get('eventRef')?.valueChanges.subscribe((e: any) => {
      if (self._evtCondition) {
        self._evtCondition.eventRef = e;
      }
    });
  }
}
