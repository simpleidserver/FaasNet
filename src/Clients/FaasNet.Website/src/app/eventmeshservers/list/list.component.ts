import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { act } from '@ngrx/effects';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { startAdd, startAddBridge, startGetAll } from '@stores/eventmeshservers/actions/eventmeshserver.actions';
import { EventMeshServerBridgeResult, EventMeshServerResult } from '@stores/eventmeshservers/models/eventmeshserver.model';
import { filter } from 'rxjs/operators';
import { MatPanelService } from '../../../components/matpanel/matpanelservice';
import { AddEventMeshServerComponent } from './add-eventmeshserver.component';
import { AddBridgeComponent, AddBridgeData } from './addbridge.component';
declare var ol: any;
declare var $: any;

@Component({
  selector: 'list-eventmeshservers',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListEventMeshServersComponent implements OnInit {
  private _map: any;
  private _markersLayer: any;
  private _linesLayer: any;
  private _overlay: any;
  private _selectedFeatures: any[] = [];
  edit: boolean = false;
  selectedEventMeshServer: EventMeshServerResult | null = null;
  isLoading: boolean = false;

  constructor(
    private store: Store<fromReducers.AppState>,
    private dialog: MatDialog,
    private actions$: ScannedActionsSubject,
    private translateService: TranslateService,
    private snackBar: MatSnackBar,
    private matPanelService : MatPanelService ) { }

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
        this.isLoading = false;
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[EventMeshServers] COMPLETE_ADD_BRIDGE'))
      .subscribe(() => {
        this.snackBar.open(this.translateService.instant('eventmeshservers.messages.eventMeshBridgeAdded'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.refresh();
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[EventMeshServers] ERROR_ADD_BRIDGE'))
      .subscribe(() => {
        this.snackBar.open(this.translateService.instant('eventmeshservers.messages.errorAddEventMeshBridge'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.isLoading = false;
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
    const markersLayerSource = new ol.source.Vector();
    const linesLayerSource = new ol.source.Vector();
    self._markersLayer = new ol.layer.Vector({
      source: markersLayerSource,
      style: new ol.style.Style({
        image: new ol.style.Icon({
          anchor: [0.5, 0.5],
          anchorXUnits: "fraction",
          anchorYUnits: "fraction",
          src: "/assets/images/marker.svg"
        })
      })
    });
    var selectStyle = new ol.style.Style({
      image: new ol.style.Icon({
        anchor: [0.5, 0.5],
        anchorXUnits: "fraction",
        anchorYUnits: "fraction",
        src: "/assets/images/marker-selected.svg"
      })
    });
    self._linesLayer = new ol.layer.Vector({
      source: linesLayerSource,
      style: new ol.style.Style({
        stroke: new ol.style.Stroke({
          color: 'green',
          width: 4
        })
      })
    });
    self._overlay = new ol.Overlay({
      element: document.getElementById("popup"),
      autoPan: true,
      autoPanAnimation: {
        duration: 250
      }
    });
    self._map.addLayer(self._markersLayer);
    self._map.addLayer(self._linesLayer);
    self._map.addOverlay(self._overlay);
    self._map.on('click', function (evt: any) {
      const feature = self._map.forEachFeatureAtPixel(evt.pixel,
        function (f: any) {
          return f;
        });
      if (feature && feature.server) {
        const coordinate = evt.coordinate;
        self.selectedEventMeshServer = feature.server;
        if (!self.edit) {
          self._overlay.setPosition(coordinate);
        } else {
          if (self._selectedFeatures.length === 1) {
            const from = self._selectedFeatures[0].server;
            const to = feature.server;
            if (to.urn == from.urn && to.port == from.port) {
              self.snackBar.open(self.translateService.instant('eventmeshservers.messages.errorAddBridgeSameNode'), self.translateService.instant('undo'), {
                duration: 2000
              });
              return;
            }

            if (from.bridges && from.bridges.filter((b : any) => b.urn == to.urn && b.port == to.port).length != 0) {
              self.snackBar.open(self.translateService.instant('eventmeshservers.messages.errorBridgeExists'), self.translateService.instant('undo'), {
                duration: 2000
              });
              return;
            }

            var data = new AddBridgeData();
            data.from = from;
            data.to = to;
            const panelRef = self.matPanelService.open(AddBridgeComponent, data);
            panelRef.closed.subscribe((e) => {
              if (!e.save) {
                self.toggleEdit();
                return;
              }

              if (!data.from || !data.to) {
                return;
              }

              self.isLoading = false;
              const action = startAddBridge({ from: data.from.urn, fromPort: data.from.port, to: data.to.urn, toPort: data.to.port });
              self.store.dispatch(action);
              self.toggleEdit();
            });
            return;
          }

          self._selectedFeatures.push(feature);
          feature.setStyle(selectStyle);
        }
      } else {
        self.selectedEventMeshServer = null;
        if (!self.edit) {
          self._overlay.setPosition(undefined);
        }
      }
    });
    self._map.on('pointermove', function (e: any) {
      const pixel = self._map.getEventPixel(e.originalEvent);
      const feature = self._map.forEachFeatureAtPixel(pixel,
        function (f: any) {
          return f;
        });
      const hit = feature && feature.server;
      document.body.style.cursor = hit ? 'pointer' : '';
    })
    this.store.pipe(select(fromReducers.selectEventMeshServersResult)).subscribe((state: EventMeshServerResult[] | null) => {
      if (!state) {
        return;
      }

      self.isLoading = false;
      self._markersLayer.getSource().clear();
      self._linesLayer.getSource().clear();
      const dic: { server: EventMeshServerResult, marker: any }[] = [];
      state.forEach((s: EventMeshServerResult) => {
        if (!s.longitude || !s.latitude) {
          return;
        }

        const nbSameUrns = state.filter(s => s.urn == s.urn).length;
        const randomNum = Math.random() * (nbSameUrns * 0.002) * (Math.round(Math.random()) ? 1 : -1);
        const longitude = s.longitude + randomNum;
        const latitude = s.latitude + randomNum;
        var marker = new ol.Feature(new ol.geom.Point(ol.proj.fromLonLat([longitude, latitude])));
        marker.server = s;
        self._markersLayer.getSource().addFeature(marker);
        dic.push({ server: s, marker: marker });
      });
      state.forEach((s: EventMeshServerResult) => {
        if (s.bridges) {
          const sourceMarker = dic.filter(r => r.server.port == s.port && r.server.urn == s.urn)[0].marker;
          const fromCoordinates = sourceMarker.getGeometry().flatCoordinates;
          s.bridges.forEach((b: EventMeshServerBridgeResult) => {
            const targetMarker = dic.filter(r => r.server.port == b.port && r.server.urn == b.urn)[0].marker;
            const toCoordinates = targetMarker.getGeometry().flatCoordinates;
            const points = [fromCoordinates, toCoordinates];
            const featureLine = new ol.Feature({
              geometry: new ol.geom.LineString(points)
            });
            self._linesLayer.getSource().addFeature(featureLine);
          });
        }
      });
    });
    this.refresh();
  }

  toggleEdit() {
    this.edit = !this.edit;
    if (!this.edit) {
      this._selectedFeatures.forEach((f) => f.setStyle());
      this._selectedFeatures = [];
    } else {
      this._overlay.setPosition(undefined);
    }
  }

  add() {
    const dialogRef = this.dialog.open(AddEventMeshServerComponent);
    dialogRef.afterClosed().subscribe((e) => {
      if (!e) {
        return;
      }

      this.isLoading = true;
      const addEventMeshServer = startAdd({ isLocalhost: e.isLocalHost, urn: e.urn, port: e.port});
      this.store.dispatch(addEventMeshServer);
    });
  }

  private refresh() {
    this.isLoading = true;
    const getAllEventMeshServers = startGetAll();
    this.store.dispatch(getAllEventMeshServers);
  }
}
