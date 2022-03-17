import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { deleteVpn, startAddVpn, startGetAllVpn } from '@stores/vpn/actions/vpn.actions';
import { filter } from 'rxjs/operators';
import { VpnResult } from '../../../stores/vpn/models/vpn.model';
import { AddVpnComponent } from './add-vpn.component';
declare var ol: any;
declare var $: any;

@Component({
  selector: 'list-applicationdomains',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListVpnComponent implements OnInit {
  displayedColumns: string[] = ['actions', 'name', 'description','createDateTime', 'updateDateTime'];
  isLoading: boolean = false;
  vpns: VpnResult[] = [];

  constructor(
    private store: Store<fromReducers.AppState>,
    private dialog: MatDialog,
    private actions$: ScannedActionsSubject,
    private translateService: TranslateService,
    private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.actions$.pipe(
      filter((action: any) => action.type === '[VPN] COMPLETE_ADD_VPN'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('vpn.messages.vpnAdded'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.isLoading = false;
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[VPN] COMPLETE_DELETE_VPN'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('vpn.messages.vpnRemoved'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.isLoading = false;
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[VPN] ERROR_DELETE_VPN'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('vpn.messages.errorRemoveVpn'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.isLoading = false;
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[VPN] ERROR_ADD_VPN'))
      .subscribe(() => {
        this.snackBar.open(this.translateService.instant('vpn.messages.errorAddVpn'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.isLoading = false;
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[VPN] ERROR_GET_ALL'))
      .subscribe(() => {
        this.isLoading = false;
      });
    this.store.pipe(select(fromReducers.selectVpnLstResult)).subscribe((state: VpnResult[] | null) => {
      if (!state) {
        return;
      }

      this.vpns = state;
      this.isLoading = false;
    });
    this.refresh();
  }

  addVpn() {
    const dialogRef = this.dialog.open(AddVpnComponent, {
      width: '800px'
    });
    dialogRef.afterClosed().subscribe((e) => {
      if (!e) {
        return;
      }

      this.isLoading = true;
      const addVpn = startAddVpn({ name: e.name, description: e.description });
      this.store.dispatch(addVpn);
    });
  }

  removeVpn(vpn: VpnResult) {
    this.isLoading = true;
    const removeVpn = deleteVpn({ name: vpn.name });
    this.store.dispatch(removeVpn);
  }

  private refresh() {
    this.isLoading = true;
    const getAllVpn = startGetAllVpn();
    this.store.dispatch(getAllVpn);
  }
}
