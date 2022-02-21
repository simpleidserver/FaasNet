import { Component } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'addeventmeshserver',
  templateUrl: './add-eventmeshserver.component.html'
})
export class AddEventMeshServerComponent{
  addEventMeshServerFormGroup: FormGroup = new FormGroup({
    isLocalHost: new FormControl(''),
    urn: new FormControl(''),
    port: new FormControl('')
  });

  constructor(
    private dialogRef: MatDialogRef<AddEventMeshServerComponent>) {
  }

  save() {
    if (!this.addEventMeshServerFormGroup.valid) {
      return;
    }

    this.dialogRef.close(this.addEventMeshServerFormGroup.value);
  }
}
