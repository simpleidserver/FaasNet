import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { startAddClient, startDeleteClient, startGetAllClients } from '@stores/vpn/actions/vpn.actions';
import { ClientResult } from '@stores/vpn/models/client.model';
import { filter } from 'rxjs/operators';
import { AddClientComponent } from './add-client.component';

@Component({
  selector: 'clients-vpn',
  templateUrl: './clients.component.html',
  styleUrls: ['./clients.component.scss']
})
export class ClientsVpnComponent implements OnInit {
  clients: ClientResult[] = [];
  displayedColumns: string[] = ['actions', 'clientId', 'purposes', 'createDateTime'];
  name: string = "";

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private actions$: ScannedActionsSubject,
    private snackBar: MatSnackBar,
    private translateService: TranslateService,
    private dialog : MatDialog  ) { }

  ngOnInit(): void {
    this.actions$.pipe(
      filter((action: any) => action.type === '[VPN] COMPLETE_ADD_CLIENT'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('vpn.messages.clientAdded'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[VPN] ERROR_ADD_CLIENT'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('vpn.messages.errorAddClient'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[VPN] COMPLETE_DELETE_CLIENT'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('vpn.messages.clientRemoved'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[VPN] ERROR_DELETE_CLIENT'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('vpn.messages.errorRemoveClient'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.store.pipe(select(fromReducers.selectClientsResult)).subscribe((state: ClientResult[] | null) => {
      if (!state) {
        return;
      }

      this.clients = state;
    });
    this.refresh();
  }

  getPurpose(purpose: number) {
    switch (purpose) {
      case 1:
        return 'Subscribe';
      case 2:
        return 'Publish';
    }

    return '';
  }

  addClient() {
    const dialogRef = this.dialog.open(AddClientComponent, {
      width : '800px'
    });
    dialogRef.afterClosed().subscribe((e) => {
      if (!e) {
        return;
      }

      const purposes = e.purposes.map((p : string) => parseInt(p));
      const act = startAddClient({ name: this.name, clientId: e.clientId, purposes: purposes });
      this.store.dispatch(act);
    });
  }

  removeClient(client: ClientResult) {
    const act = startDeleteClient({ clientId: client.clientId, name: this.name });
    this.store.dispatch(act);
  }

  private refresh() {
    this.name = this.activatedRoute.parent?.snapshot.params['name'];
    const act = startGetAllClients({ name: this.name });
    this.store.dispatch(act);
  }
}
