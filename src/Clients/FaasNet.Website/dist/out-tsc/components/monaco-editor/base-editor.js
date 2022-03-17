import { __decorate, __param } from "tslib";
import { Component, EventEmitter, Inject, Output, ViewChild } from '@angular/core';
import { NGX_MONACO_EDITOR_CONFIG } from './config';
let loadedMonaco = false;
let loadPromise;
let BaseEditor = class BaseEditor {
    constructor(config) {
        this.config = config;
        this.onInit = new EventEmitter();
        this._editorContainer = null;
        this._windowResizeSubscription = null;
    }
    ngAfterViewInit() {
        if (loadedMonaco) {
            loadPromise.then(() => {
                this.initMonaco(this._options);
            });
        }
        else {
            loadedMonaco = true;
            loadPromise = new Promise((resolve) => {
                const baseUrl = (this.config.baseUrl || './assets') + '/monaco-editor/min/vs';
                if (typeof (window.monaco) === 'object') {
                    resolve();
                    return;
                }
                const onGotAmdLoader = () => {
                    window.require.config({ paths: { 'vs': `${baseUrl}` } });
                    window.require([`vs/editor/editor.main`], () => {
                        if (typeof this.config.onMonacoLoad === 'function') {
                            this.config.onMonacoLoad();
                        }
                        this.initMonaco(this._options);
                        resolve();
                    });
                };
                if (!window.require) {
                    const loaderScript = document.createElement('script');
                    loaderScript.type = 'text/javascript';
                    loaderScript.src = `${baseUrl}/loader.js`;
                    loaderScript.addEventListener('load', onGotAmdLoader);
                    document.body.appendChild(loaderScript);
                }
                else {
                    onGotAmdLoader();
                }
            });
        }
    }
    ngOnDestroy() {
        if (this._windowResizeSubscription) {
            this._windowResizeSubscription.unsubscribe();
        }
        if (this._editor) {
            this._editor.dispose();
            this._editor = undefined;
        }
    }
};
__decorate([
    ViewChild('editorContainer', { static: true })
], BaseEditor.prototype, "_editorContainer", void 0);
__decorate([
    Output()
], BaseEditor.prototype, "onInit", void 0);
BaseEditor = __decorate([
    Component({
        template: ''
    }),
    __param(0, Inject(NGX_MONACO_EDITOR_CONFIG))
], BaseEditor);
export { BaseEditor };
//# sourceMappingURL=base-editor.js.map