import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { deleteVpn } from '@stores/vpn/actions/vpn.actions';
import { VpnResult } from '@stores/vpn/models/vpn.model';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'info-vpn',
  templateUrl: './info.component.html'
})
export class InfoVpnComponent implements OnInit, OnDestroy {
  vpn: VpnResult | undefined;

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private actions$: ScannedActionsSubject,
    private translateService: TranslateService,
    private snackBar: MatSnackBar,
    private router: Router) { }

  ngOnInit(): void {
    this.actions$.pipe(
      filter((action: any) => action.type === '[VPN] COMPLETE_DELETE_VPN'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('vpn.messages.vpnRemoved'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.router.navigate(['/vpns']);
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[VPN] ERROR_DELETE_VPN'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('vpn.messages.errorRemoveVpn'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.store.pipe(select(fromReducers.selectVpnResult)).subscribe((state: VpnResult | null) => {
      if (!state) {
        return;
      }

      this.vpn = state;
    });
  }

  ngOnDestroy() {
  }

  delete() {
    const name = this.activatedRoute.parent?.snapshot.params['name'];
    const action = deleteVpn({ name: name });
    this.store.dispatch(action);
  }
}
