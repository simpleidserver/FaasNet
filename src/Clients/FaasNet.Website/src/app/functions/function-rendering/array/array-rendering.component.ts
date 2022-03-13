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

    this.form = form;
    if (!this.form?.contains(this.option.Name)) {
      const formArr = new FormArray([]);
      this.form.addControl(this.option.Name, formArr);
    } else {
      const formArr = this.form.controls[this.option.Name] as FormArray;
      formArr.controls.forEach((r: any) => {
        this.children.push({
          form: r,
          parameters: this.option.Parameters
        });
      });
    }
  }
}
