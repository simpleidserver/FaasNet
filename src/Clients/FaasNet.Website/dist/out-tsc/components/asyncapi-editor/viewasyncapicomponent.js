import { __decorate } from "tslib";
import { Component } from "@angular/core";
import { MatPanelContent } from "../matpanel/matpanelcontent";
import { AsyncApiBuilder } from "./builders/asyncapibuilder";
export class ViewAsyncApiData {
    constructor() {
        this.application = null;
        this.consumedLinks = [];
    }
}
let ViewAsyncApiComponent = class ViewAsyncApiComponent extends MatPanelContent {
    constructor() {
        super();
        this.jsonOptions = { theme: 'vs', language: 'json', automaticLayout: true };
        this.json = "";
    }
    init(data) {
        var viewAsyncApiData = data;
        if (!viewAsyncApiData || !viewAsyncApiData.application) {
            return;
        }
        var json = AsyncApiBuilder.build(viewAsyncApiData.application, viewAsyncApiData.consumedLinks);
        this.json = JSON.stringify(json, null, "\t");
    }
};
ViewAsyncApiComponent = __decorate([
    Component({
        selector: 'viewasyncapi',
        templateUrl: './viewasyncapi.component.html',
        styleUrls: ['./viewasyncapi.component.scss']
    })
], ViewAsyncApiComponent);
export { ViewAsyncApiComponent };
//# sourceMappingURL=viewasyncapicomponent.js.map