import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { ServerStatusResult } from '@stores/server/models/serverstatus.model';
import { startGetAllVpn } from '@stores/vpn/actions/vpn.actions';
import { VpnResult } from '@stores/vpn/models/vpn.model';
import { startGetServerStatus } from '../stores/server/actions/server.actions';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  vpnLstResult: VpnResult[] = [];
  serverStatus: ServerStatusResult = new ServerStatusResult();
  vpnFormControl: FormControl = new FormControl();

  constructor(
    private translate: TranslateService,
    private store: Store<fromReducers.AppState> ) {
    this.translate.setDefaultLang('en');
    this.translate.use('en');
  }

  ngOnInit() {
    this.store.pipe(select(fromReducers.selectServerStatusResult)).subscribe((serverStatus: ServerStatusResult | null) => {
      if (!serverStatus || serverStatus.isRunning === false) {
        return;
      }

      this.serverStatus = serverStatus;
      const getAllVpn = startGetAllVpn();
      this.store.dispatch(getAllVpn);
    });
    this.store.pipe(select(fromReducers.selectVpnLstResult)).subscribe((vpnLstResult: VpnResult[] | null) => {
      if (!vpnLstResult) {
        return;
      }

      this.vpnLstResult = vpnLstResult;
    });

    this.vpnFormControl.valueChanges.subscribe(() => {

    });
    const getServerStatus = startGetServerStatus();
    this.store.dispatch(getServerStatus);
  }

  ngAfterViewInit() {
  }
}