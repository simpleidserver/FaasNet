import { __decorate, __param } from "tslib";
import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
let TokenComponent = class TokenComponent {
    constructor(data) {
        this.data = data;
        this.json = "";
        this.jsonOptions = { theme: 'vs', language: 'json', automaticLayout: true };
        this.json = JSON.stringify(data, null, "\t");
    }
};
TokenComponent = __decorate([
    Component({
        selector: 'display-token-json',
        templateUrl: './token.component.html',
        styleUrls: [
            './token.component.scss',
        ]
    }),
    __param(0, Inject(MAT_DIALOG_DATA))
], TokenComponent);
export { TokenComponent };
//# sourceMappingURL=token.component.js.map