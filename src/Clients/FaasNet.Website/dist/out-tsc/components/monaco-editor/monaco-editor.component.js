var EditorComponent_1;
import { __decorate, __param } from "tslib";
import { Component, forwardRef, Inject, Input } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { fromEvent } from 'rxjs';
import { BaseEditor } from './base-editor';
import { NGX_MONACO_EDITOR_CONFIG } from './config';
import { JQLanguage } from './languages/jq/jq-contribution';
let EditorComponent = EditorComponent_1 = class EditorComponent extends BaseEditor {
    constructor(zone, editorConfig) {
        super(editorConfig);
        this.zone = zone;
        this.editorConfig = editorConfig;
        this._value = '';
        this.isSingleLine = false;
        this.customSize = "medium";
        this.propagateChange = (_) => { };
        this.onTouched = () => { };
    }
    set options(options) {
        let defaultOptions = this.config.defaultOptions;
        if (options.isSingleLine && options.isSingleLine === true) {
            defaultOptions = {
                wordWrap: 'off',
                lineNumbers: 'off',
                lineNumbersMinChars: 0,
                overviewRulerLanes: 0,
                overviewRulerBorder: false,
                hideCursorInOverviewRuler: true,
                lineDecorationsWidth: 0,
                glyphMargin: false,
                folding: false,
                scrollBeyondLastColumn: 0,
                scrollbar: {
                    horizontal: 'hidden',
                    vertical: 'hidden',
                    alwaysConsumeMouseWheel: false,
                },
                find: {
                    addExtraSpaceOnTop: false,
                    autoFindInSelection: 'never',
                    seedSearchStringFromSelection: false,
                },
                minimap: { enabled: false },
                wordBasedSuggestions: false,
                links: false,
                occurrencesHighlight: false,
                cursorStyle: 'line-thin',
                renderLineHighlight: 'none',
                contextmenu: false,
                roundedSelection: false,
                hover: {
                    delay: 100,
                },
                acceptSuggestionOnEnter: 'on',
                automaticLayout: true,
                fixedOverflowWidgets: true
            };
            delete options['isSingleLine'];
            this.isSingleLine = true;
            this.customSize = "small";
        }
        else if (options.customSize) {
            this.customSize = options.customSize;
            delete options["customSize"];
        }
        this._options = Object.assign({}, defaultOptions, options);
        if (this._editor) {
            this._editor.dispose();
            this.initMonaco(options);
        }
    }
    get options() {
        return this._options;
    }
    set model(model) {
        this.options.model = model;
        if (this._editor) {
            this._editor.dispose();
            this.initMonaco(this.options);
        }
    }
    writeValue(value) {
        this._value = value || '';
        setTimeout(() => {
            if (this._editor && !this.options.model) {
                this._editor.setValue(this._value);
            }
        });
    }
    registerOnChange(fn) {
        this.propagateChange = fn;
    }
    registerOnTouched(fn) {
        this.onTouched = fn;
    }
    initMonaco(options) {
        var _a;
        const hasModel = !!options.model;
        this.loadLanguages();
        if (hasModel) {
            const model = monaco.editor.getModel(options.model.uri || '');
            if (model) {
                options.model = model;
                options.model.setValue(this._value);
            }
            else {
                options.model = monaco.editor.createModel(options.model.value, options.model.language, options.model.uri);
            }
        }
        this._editor = monaco.editor.create((_a = this._editorContainer) === null || _a === void 0 ? void 0 : _a.nativeElement, options);
        if (!hasModel) {
            this._editor.setValue(this._value);
        }
        this._editor.onDidChangeModelContent((e) => {
            const value = this._editor.getValue();
            this.zone.run(() => {
                this.propagateChange(value);
                this._value = value;
            });
        });
        this._editor.onDidBlurEditorWidget(() => {
            this.onTouched();
        });
        if (this.isSingleLine) {
            this._editor.onKeyDown((e) => {
                if (e.keyCode === monaco.KeyCode.Enter) {
                    if (this._editor.getContribution('editor.contrib.suggestController').model.state == 0) {
                        e.preventDefault();
                    }
                }
            });
        }
        if (this._windowResizeSubscription) {
            this._windowResizeSubscription.unsubscribe();
        }
        this._windowResizeSubscription = fromEvent(window, 'resize').subscribe(() => this._editor.layout());
        this.onInit.emit(this._editor);
    }
    loadLanguages() {
        if (!EditorComponent_1.languagesLoaded) {
            JQLanguage.import();
            EditorComponent_1.languagesLoaded = true;
        }
    }
};
EditorComponent.languagesLoaded = false;
__decorate([
    Input('options')
], EditorComponent.prototype, "options", null);
__decorate([
    Input('model')
], EditorComponent.prototype, "model", null);
EditorComponent = EditorComponent_1 = __decorate([
    Component({
        selector: 'ngx-monaco-editor',
        templateUrl: './monaco-editor.component.html',
        styleUrls: ['./monaco-editor.component.scss'],
        providers: [{
                provide: NG_VALUE_ACCESSOR,
                useExisting: forwardRef(() => EditorComponent_1),
                multi: true
            }]
    }),
    __param(1, Inject(NGX_MONACO_EDITOR_CONFIG))
], EditorComponent);
export { EditorComponent };
//# sourceMappingURL=monaco-editor.component.js.map