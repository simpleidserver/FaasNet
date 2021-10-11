import { Component } from "@angular/core";
import { FormArray, FormGroup } from "@angular/forms";
import { BaseRenderingComponent } from "../base-rendering.component";

@Component({
  selector: 'view-array',
  templateUrl: 'array-rendering.component.html',
  styleUrls: ['./array-rendering.component.scss']
})
export class ArrayRenderingComponent extends BaseRenderingComponent {
  children: any[] = [];
  isLoaded: boolean = true;

  addParameter(evt: any) {
    evt.preventDefault();
    const formGroup = new FormGroup({});
    const formArr = this.form?.get(this.option.Name) as FormArray;
    formArr.push(formGroup);
    this.children.push({
      form: formGroup,
      parameters: this.option.Parameters
    });
  }

  removeParameter(evt: any, child: any) {
    evt.preventDefault();
    const formArr = this.form?.get(this.option.Name) as FormArray;
    const index = this.children.indexOf(child);
    formArr.removeAt(formArr.controls.indexOf(child.form));
    this.children.splice(index, 1);
  }

  setForm(form: FormGroup | null) {
    if (!form) {
      return;
    }

    const formArr = new FormArray([]);
    this.form = form;
    this.form.addControl(this.option.Name, formArr);
  }
}
