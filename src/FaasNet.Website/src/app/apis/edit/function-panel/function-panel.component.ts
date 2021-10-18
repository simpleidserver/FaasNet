import { Component, Input } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { FunctionResult } from "@stores/functions/models/function.model";
import { AddFunctionComponent } from "./add-function.component";
import { FunctionModel, FunctionRecord } from "./function.model";
import { UpdateFunctionConfigurationComponent } from "./update-configuration.component";

@Component({
  selector: 'function-panel',
  templateUrl: './function-panel.component.html',
  styleUrls: ['./function-panel.component.scss']
})
export class FunctionPanelComponent {
  @Input() model: FunctionModel | null = null;

  constructor(
    private dialog: MatDialog) { }

  chooseFunction() {
    const dialogRef = this.dialog.open(AddFunctionComponent, {
      width: '800px'
    });
    dialogRef.afterClosed().subscribe((opt: FunctionResult) => {
      if (!opt || !opt.name || !this.model) {
        return;
      }

      let record = new FunctionRecord();
      record.info = opt;
      this.model.content = record;
    });
  }

  updateConfiguration() {
    const dialogRef = this.dialog.open(UpdateFunctionConfigurationComponent, {
      data: this.model?.content,
      width: '800px'
    });
    dialogRef.afterClosed().subscribe((opt: any) => {
      if (!opt || !this.model || !this.model.content) {
        return;
      }

      this.model.content.configuration = opt;
    });
  }

  isSelected() {
    return this.model && this.model.content && this.model.content.info && this.model.content.info.name;
  }
}
