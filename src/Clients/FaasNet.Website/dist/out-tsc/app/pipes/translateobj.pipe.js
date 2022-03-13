import { __decorate } from "tslib";
import { Injectable, Pipe } from "@angular/core";
let TranslateObjPipe = class TranslateObjPipe {
    constructor(ts) {
        this.ts = ts;
    }
    transform(translations) {
        if (!translations || translations.length === 0) {
            return null;
        }
        const lng = this.ts.currentLang;
        const filteredTranslations = translations.filter((t) => {
            return t.Language === lng;
        });
        if (filteredTranslations.length !== 1) {
            return "unknown";
        }
        return filteredTranslations[0].Description;
    }
};
TranslateObjPipe = __decorate([
    Injectable(),
    Pipe({
        name: 'translateobj',
        pure: false
    })
], TranslateObjPipe);
export { TranslateObjPipe };
//# sourceMappingURL=translateobj.pipe.js.map