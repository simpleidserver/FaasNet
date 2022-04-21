import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { NavigationEnd, Router } from '@angular/router';
import { select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { ServerStatusResult } from '@stores/server/models/serverstatus.model';
import { startGetAllVpn, selectVpn } from '@stores/vpn/actions/vpn.actions';
import { VpnResult } from '@stores/vpn/models/vpn.model';
import { startGetServerStatus } from '../stores/server/actions/server.actions';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  isUpdated: boolean = false;
  vpnLstResult: VpnResult[] = [];
  serverStatus: ServerStatusResult = new ServerStatusResult();
  vpnFormControl: FormControl = new FormControl();
  showLogging: boolean = false;

  constructor(
    private translate: TranslateService,
    private store: Store<fromReducers.AppState>,
    private router: Router) {
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
    this.store.pipe(select(fromReducers.selectActiveVpnResult)).subscribe((activeVpn: string | null) => {
      if (!activeVpn || this.isUpdated) {
        return;
      }

      this.vpnFormControl.setValue(activeVpn);
    });
    this.vpnFormControl.valueChanges.subscribe(() => {
      const act = selectVpn({ name: this.vpnFormControl.value });
      this.store.dispatch(act);
    });
    const getServerStatus = startGetServerStatus();
    this.router.events.subscribe((e) => {
      if (e instanceof NavigationEnd) {
        if (this.router.routerState.snapshot.url.startsWith('/logging')) {
          this.showLogging = true;
        }
      }
    });
    this.store.dispatch(getServerStatus);
  }

  ngAfterViewInit() {
  }
}
