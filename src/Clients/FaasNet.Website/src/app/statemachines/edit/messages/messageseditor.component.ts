import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ScannedActionsSubject, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import * as fromReducers from '@stores/appstate';

@Component({
  selector: 'messageseditor-statemachine',
  templateUrl: './messageseditor.component.html',
  styleUrls: ['./messageseditor.component.scss']
})
export class MessagesEditorComponent implements OnInit {

  constructor(
    private store: Store<fromReducers.AppState>,
    private actions$: ScannedActionsSubject,
    private snackBar: MatSnackBar,
    private translateService: TranslateService) {
  }

  ngOnInit(): void {
  }
}
