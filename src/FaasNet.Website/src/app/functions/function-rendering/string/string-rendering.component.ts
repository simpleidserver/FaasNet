import { Component } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { BaseRenderingComponent } from "../base-rendering.component";

@Component({
  selector: 'view-string',
  templateUrl: 'string-rendering.component.html',
  styleUrls: ['./string-rendering.component.scss']
})
export class StringRenderingComponent extends BaseRenderingComponent {
  control: FormControl = new FormControl();

  setForm(form: FormGroup | null) {
    if (!form) {
      return;
    }

    this.form = form;
    this.form.addControl(this.option.Name, this.control);
  }
}
