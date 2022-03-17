import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { select } from '@ngrx/store';
import { startAddApplicationDomain, startGetAllApplicationDomains } from '@stores/application/actions/application.actions';
import * as fromReducers from '@stores/appstate';
import { filter } from 'rxjs/operators';
import { AddApplicationDomainComponent } from './add-applicationdomain.component';
let ListApplicationDomainComponent = class ListApplicationDomainComponent {
    constructor(store, dialog, actions$, translateService, snackBar) {
        this.store = store;
        this.dialog = dialog;
        this.actions$ = actions$;
        this.translateService = translateService;
        this.snackBar = snackBar;
        this.displayedColumns = ['name', 'description', 'version', 'createDateTime', 'updateDateTime'];
        this.isLoading = false;
        this.applicationDomains = [];
    }
    ngOnInit() {
        this.actions$.pipe(filter((action) => action.type === '[Applications] COMPLETE_ADD_APPLICATION_DOMAIN'))
            .subscribe((e) => {
            this.snackBar.open(this.translateService.instant('applicationDomains.messages.applicationDomainAdded'), this.translateService.instant('undo'), {
                duration: 2000
            });
            this.isLoading = false;
        });
        this.actions$.pipe(filter((action) => action.type === '[Applications] ERROR_ADD_APPLICATION_DOMAIN'))
            .subscribe(() => {
            this.snackBar.open(this.translateService.instant('applicationDomains.messages.errorAddApplicationDomain'), this.translateService.instant('undo'), {
                duration: 2000
            });
            this.isLoading = false;
        });
        this.store.pipe(select(fromReducers.selectApplicationDomainsResult)).subscribe((state) => {
            if (!state) {
                return;
            }
            this.applicationDomains = state;
            this.isLoading = false;
        });
        this.refresh();
    }
    addApplicationDomain() {
        const dialogRef = this.dialog.open(AddApplicationDomainComponent);
        dialogRef.afterClosed().subscribe((e) => {
            if (!e) {
                return;
            }
            this.isLoading = true;
            const addApplicationDomain = startAddApplicationDomain({ rootTopic: e.rootTopic, name: e.name, description: e.description });
            this.store.dispatch(addApplicationDomain);
        });
    }
    refresh() {
        this.isLoading = true;
        const getAllApplicationDomain = startGetAllApplicationDomains();
        this.store.dispatch(getAllApplicationDomain);
    }
};
ListApplicationDomainComponent = __decorate([
    Component({
        selector: 'list-applicationdomains',
        templateUrl: './list.component.html',
        styleUrls: ['./list.component.scss']
    })
], ListApplicationDomainComponent);
export { ListApplicationDomainComponent };
//# sourceMappingURL=list.component.js.map