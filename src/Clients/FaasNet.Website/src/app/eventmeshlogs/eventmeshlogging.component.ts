import { Component } from '@angular/core';
import { environment } from '@envs/environment';

@Component({
  selector: 'eventmeshlogging',
  templateUrl: './eventmeshlogging.component.html',
  styleUrls: ['./eventmeshlogging.component.scss']
})
export class EventMeshLoggingComponent {
  eventMeshLogsUrl: string = environment.eventMeshLogsUrl;
  eventMeshDashboardUrl: string = environment.eventMeshDashboardUrl;
  constructor() { }
}
