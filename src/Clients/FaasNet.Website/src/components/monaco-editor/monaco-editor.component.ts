import { Component, forwardRef, Inject, Input, NgZone } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { fromEvent } from 'rxjs';
import { BaseEditor } from './base-editor';
import { NgxMonacoEditorConfig, NGX_MONACO_EDITOR_CONFIG } from './config';
import { JQLanguage } from './languages/jq/jq-contribution';
import { NgxEditorModel } from './types';

declare var monaco: any;

@Component({
  selector: 'ngx-monaco-editor',
  templateUrl: './monaco-editor.component.html',
  styleUrls: ['./monaco-editor.component.scss'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => EditorComponent),
    multi: true
  }]
})
export class EditorComponent extends BaseEditor implements ControlValueAccessor {
  private _value: string = '';
  private static languagesLoaded: boolean = false;
  isSingleLine: boolean = false;

  customSize: string = "medium";

  propagateChange = (_: any) => { };
  onTouched = () => { };

  @Input('options')
  set options(options: any) {
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
    } else if (options.customSize) {
      this.customSize = options.customSize;
      delete options["customSize"];
    }

    this._options = Object.assign({}, defaultOptions, options);
    if (this._editor) {
      this._editor.dispose();
      this.initMonaco(options);
    }
  }

  get options(): any {
    return this._options;
  }

  @Input('model')
  set model(model: NgxEditorModel) {
    this.options.model = model;
    if (this._editor) {
      this._editor.dispose();
      this.initMonaco(this.options);
    }
  }

  constructor(private zone: NgZone, @Inject(NGX_MONACO_EDITOR_CONFIG) private editorConfig: NgxMonacoEditorConfig) {
    super(editorConfig);
  }

  writeValue(value: any): void {
    this._value = value || '';
    setTimeout(() => {
      if (this._editor && !this.options.model) {
        this._editor.setValue(this._value);
      }
    });
  }

  registerOnChange(fn: any): void {
    this.propagateChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  protected initMonaco(options: any): void {
    const hasModel = !!options.model;
    this.loadLanguages();
    if (hasModel) {
      const model = monaco.editor.getModel(options.model.uri || '');
      if (model) {
        options.model = model;
        options.model.setValue(this._value);
      } else {
        options.model = monaco.editor.createModel(options.model.value, options.model.language, options.model.uri);
      }
    }

    this._editor = monaco.editor.create(this._editorContainer?.nativeElement, options);
    if (!hasModel) {
      this._editor.setValue(this._value);
    }

    this._editor.onDidChangeModelContent((e: any) => {
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
      this._editor.onKeyDown((e: any) => {
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

  private loadLanguages(): void {
    if (!EditorComponent.languagesLoaded) {
      JQLanguage.import();
      EditorComponent.languagesLoaded = true;
    }
  }
}
