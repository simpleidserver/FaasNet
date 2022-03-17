import { __decorate } from "tslib";
import { Component } from '@angular/core';
let EditDomainComponent = class EditDomainComponent {
    constructor(route) {
        this.route = route;
        this.isLoading = false;
        this.id = "";
    }
    ngOnInit() {
        this.id = this.route.snapshot.params['id'];
        console.log(this.id);
    }
};
EditDomainComponent = __decorate([
    Component({
        selector: 'edit-domain',
        templateUrl: './edit.component.html',
        styleUrls: ['./edit.component.scss']
    })
], EditDomainComponent);
export { EditDomainComponent };
//# sourceMappingURL=edit.component.js.map