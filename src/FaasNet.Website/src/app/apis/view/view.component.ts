import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { ScannedActionsSubject, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';
import { Subscription } from 'rxjs';

@Component({
  selector: 'view-api',
  templateUrl: './view.component.html'
})
export class ViewApiDefComponent implements OnInit, OnDestroy {
  name: string | undefined;
  subscription: Subscription | undefined;

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private actions$: ScannedActionsSubject,
    private translateService: TranslateService,
    private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.subscription = this.activatedRoute.params.subscribe(() => {
      this.refresh();
    });
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  private refresh() {
    const name = this.activatedRoute.snapshot.params['name'];
    this.name = name;
  }
}
