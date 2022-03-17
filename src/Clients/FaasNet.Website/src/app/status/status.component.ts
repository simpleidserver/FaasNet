import { Component, OnDestroy, OnInit } from '@angular/core';
import { ScannedActionsSubject, Store } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { startGetServerStatus } from '@stores/server/actions/server.actions';
import { ServerStatusResult } from '@stores/server/models/serverstatus.model';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'server-status',
  templateUrl: './status.component.html',
  styleUrls: ['./status.component.scss']
})
export class ServerStatusComponent implements OnInit, OnDestroy {
  serverStatus: ServerStatusResult = new ServerStatusResult();
  lastRefreshTime: Date | null = null;
  interval: any;

  constructor(
    private store: Store<fromReducers.AppState>,
    private actions$: ScannedActionsSubject) {
  }

  ngOnInit() {
    const self = this;
    this.actions$.pipe(
      filter((action: any) => action.type === '[SERVER] COMPLETE_GET_STATUS'))
      .subscribe((e) => {
        self.serverStatus = e.content;
        self.lastRefreshTime = new Date();
      });
    self.interval = setInterval(function () {
      self.refresh();
    }, 5000);
    self.refresh();
  }

  ngOnDestroy() {
    if (this.interval) {
      clearInterval(this.interval);
    }
  }

  private refresh(): void {
    const action = startGetServerStatus();
    this.store.dispatch(action);
  }
}
