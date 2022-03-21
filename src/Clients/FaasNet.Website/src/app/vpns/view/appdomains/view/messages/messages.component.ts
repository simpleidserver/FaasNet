import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { select, Store } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { getLatestMessages } from '@stores/vpn/actions/vpn.actions';
import { MessageDefinitionResult } from '@stores/vpn/models/messagedefinition.model';

@Component({
  selector: 'messages-vpn',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.scss']
})
export class MessagesVpnComponent implements OnInit {
  messages: MessageDefinitionResult[] = [];
  displayedColumns: string[] = ['actions', 'name', 'description', 'version', 'createDateTime', 'updateDateTime'];

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.store.pipe(select(fromReducers.selectMessageDefsResult)).subscribe((state: MessageDefinitionResult[] | null) => {
      if (!state) {
        return;
      }

      this.messages = state;
    });
    this.refresh();
  }

  removeMessage(message: MessageDefinitionResult) {

  }

  addMessage() {

  }

  private refresh() {
    const vpnName = this.activatedRoute.parent?.snapshot.params['vpnName'];
    const appDomainId = this.activatedRoute.parent?.snapshot.params['appDomainId'];
    const act = getLatestMessages({ name: vpnName, appDomainId: appDomainId});
    this.store.dispatch(act);
  }
}
