import { __decorate } from "tslib";
import { Component } from '@angular/core';
import { startGetServerStatus } from '@stores/server/actions/server.actions';
import { ServerStatusResult } from '@stores/server/models/serverstatus.model';
import { filter } from 'rxjs/operators';
let ServerStatusComponent = class ServerStatusComponent {
    constructor(store, actions$) {
        this.store = store;
        this.actions$ = actions$;
        this.serverStatus = new ServerStatusResult();
        this.lastRefreshTime = null;
    }
    ngOnInit() {
        const self = this;
        this.actions$.pipe(filter((action) => action.type === '[SERVER] COMPLETE_GET_STATUS'))
            .subscribe((e) => {
            self.serverStatus = e.content;
            self.lastRefreshTime = new Date();
        });
        self.interval = setInterval(function () {
            self.refresh();
        }, 5000);
        self.refresh();
    }
    ngOnDestroy() {
        if (this.interval) {
            clearInterval(this.interval);
        }
    }
    refresh() {
        const action = startGetServerStatus();
        this.store.dispatch(action);
    }
};
ServerStatusComponent = __decorate([
    Component({
        selector: 'server-status',
        templateUrl: './status.component.html',
        styleUrls: ['./status.component.scss']
    })
], ServerStatusComponent);
export { ServerStatusComponent };
//# sourceMappingURL=status.component.js.map