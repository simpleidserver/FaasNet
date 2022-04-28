import { Component } from '@angular/core';
import { environment } from '@envs/environment';

@Component({
  selector: 'statemachinelogging',
  templateUrl: './statemachinelogging.component.html',
  styleUrls: ['./statemachinelogging.component.scss']
})
export class StateMachineLoggingComponent {
  stateMachineLogsUrl: string = environment.stateMachineLogsUrl;
  stateMachineDashboardUrl: string = environment.stateMachineDashboardUrl;
  constructor() { }
}
