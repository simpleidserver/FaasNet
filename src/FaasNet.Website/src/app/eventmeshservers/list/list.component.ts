import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { startAdd, startGetAll } from '@stores/eventmeshservers/actions/eventmeshserver.actions';
import { EventMeshServerResult } from '@stores/eventmeshservers/models/eventmeshserver.model';
import { filter } from 'rxjs/operators';
import { AddEventMeshServerComponent } from './add-eventmeshserver.component';
declare var ol: any;

@Component({
  selector: 'list-eventmeshservers',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListEventMeshServersComponent implements OnInit {
  private _map: any;
  private _markersLayer : any;

  constructor(
    private store: Store<fromReducers.AppState>,
    private dialog: MatDialog,
    private actions$: ScannedActionsSubject,
    private translateService: TranslateService,
    private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    const self = this;
    this.actions$.pipe(
      filter((action: any) => action.type === '[EventMeshServers] COMPLETE_ADD_EVENTMESH_SERVER'))
      .subscribe((e) => {
        this.snackBar.open(this.translateService.instant('eventmeshservers.messages.eventMeshServerAdded'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.refresh();
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[EventMeshServers] ERROR_ADD_EVENTMESH_SERVER'))
      .subscribe(() => {
        this.snackBar.open(this.translateService.instant('eventmeshservers.messages.errorAddEventMeshServer'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    navigator.geolocation.getCurrentPosition(function (pos) {
      const crd = pos.coords;
      self._map.getView().setCenter(ol.proj.fromLonLat([crd.longitude, crd.latitude]));
      self._map.getView().setZoom(6);
    });
    self._map = new ol.Map({
      target: 'map',
      layers: [
        new ol.layer.Tile({
          source: new ol.source.OSM()
        })
      ],
      view: new ol.View({
        center: ol.proj.fromLonLat([37.41, 8.82]),
        zoom: 4
      })
    });
    self._markersLayer = new ol.layer.Vector({
      source: new ol.source.Vector()
    });
    self._map.addLayer(self._markersLayer);
    this.store.pipe(select(fromReducers.selectEventMeshServersResult)).subscribe((state: EventMeshServerResult[] | null) => {
      if (!state) {
        return;
      }

      state.forEach((s: EventMeshServerResult) => {
        var marker = new ol.Feature(new ol.geom.Point(ol.proj.fromLonLat([s.longitude, s.latitude])));
        self._markersLayer.getSource().addFeature(marker);
      });
    });
    this.refresh();
  }

  add() {
    const dialogRef = this.dialog.open(AddEventMeshServerComponent);
    dialogRef.afterClosed().subscribe((e) => {
      if (!e) {
        return;
      }

      const addEventMeshServer = startAdd({ isLocalhost: e.isLocalHost, urn: e.urn, port: e.port});
      this.store.dispatch(addEventMeshServer);
    });
  }

  private refresh() {
    const getAllEventMeshServers = startGetAll();
    this.store.dispatch(getAllEventMeshServers);
  }
}
