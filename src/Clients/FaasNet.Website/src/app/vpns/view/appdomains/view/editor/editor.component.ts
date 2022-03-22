import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { select, Store } from '@ngrx/store';
import * as fromReducers from '@stores/appstate';
import { AppDomainResult } from '@stores/vpn/models/appdomain.model';

@Component({
  selector: 'editor-domain',
  templateUrl: './editor.component.html',
  styleUrls: ['./editor.component.scss']
})
export class EditorDomainComponent implements OnInit {
  vpnName: string = "";
  appDomainId: string = "";
  rootTopic: string = "";

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.store.pipe(select(fromReducers.selectAppDomainResult)).subscribe((state: AppDomainResult | null) => {
      if (!state) {
        return;
      }

      this.rootTopic = state.rootTopic;
    });
    this.vpnName = this.activatedRoute.parent?.snapshot.params['vpnName'];
    this.appDomainId = this.activatedRoute.parent?.snapshot.params['appDomainId'];
  }
}
