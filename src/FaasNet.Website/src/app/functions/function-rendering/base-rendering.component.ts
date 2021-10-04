import { Component, EventEmitter, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";

@Component({
  template: ''
})
export class BaseRenderingComponent implements OnInit {
  onInitialized = new EventEmitter();
  option: any;
  form: FormGroup | null = null;

  constructor() { }

  ngOnInit() {
    this.onInitialized.emit();
  }

  setForm(form: FormGroup | null) {
  }
}
