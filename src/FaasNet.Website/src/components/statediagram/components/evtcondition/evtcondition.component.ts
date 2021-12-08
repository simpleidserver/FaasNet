import { Component, Input } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { BaseTransition } from "@stores/statemachines/models/statemachine-state.model";
import { EventCondition } from "@stores/statemachines/models/statemachine-switch-state.model";

@Component({
  selector: 'evtcondition-editor',
  templateUrl: './evtcondition.component.html',
  styleUrls: ['../state-editor.component.scss']
})
export class EvtConditionComponent {
  private eventRefSubscription: any | null = null;
  private _evtCondition: EventCondition | null = null;
  private _transition: BaseTransition | null = null;
  @Input()
  get transition(): BaseTransition | null {
    return this._transition;
  }
  set transition(v: BaseTransition | null) {
    this._transition = v;
    this._evtCondition = v as EventCondition;
    this.init();
  }

  updateEvtConditionFormGroup: FormGroup = new FormGroup({
    eventRef: new FormControl()
  });

  constructor() {
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    if (this.eventRefSubscription) {
      this.eventRefSubscription.unsubscribe();
    }
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
