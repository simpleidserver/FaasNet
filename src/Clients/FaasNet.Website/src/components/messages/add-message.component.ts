import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MessageDefinitionResult } from '@stores/messagedefinitions/models/messagedefinition.model';

@Component({
  selector: 'add-messagedef',
  templateUrl: './add-message.component.html'
})
export class AddMessageDefComponent implements OnInit, OnDestroy {
  private listener: any;
  private cloudEvtProperties: string[] = ['id', 'source', 'type']
  addMessageDefFormGroup: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required]),
    description: new FormControl('', [Validators.required]),
    messageJsonSchema: new FormControl(''),
    jsonSchema: new FormControl()
  });
  messageJsonOptions: any = { theme: 'vs', language: 'json' };
  jsonOptions: any = { theme: 'vs', language: 'json', readOnly: true };
  isEditable: boolean = false;

  constructor(
    private dialogRef: MatDialogRef<AddMessageDefComponent>,
    @Inject(MAT_DIALOG_DATA) public data: MessageDefinitionResult) {
    if (data) {
      this.addMessageDefFormGroup.get('name')?.setValue(data.name);
      this.addMessageDefFormGroup.get('description')?.setValue(data.description);
      const messageJsonSchema = this.extractJsonMessage(data.jsonSchema);
      this.addMessageDefFormGroup.get('messageJsonSchema')?.setValue(messageJsonSchema);
      this.refreshJsonSchema();
      this.isEditable = true;
    }
  }

  save() {
    if (!this.addMessageDefFormGroup.valid) {
      return;
    }

    this.dialogRef.close(this.addMessageDefFormGroup.value);
  }

  ngOnInit() {
    const self = this;
    self.listener = self.addMessageDefFormGroup.get('messageJsonSchema')?.valueChanges.subscribe(() => {
      self.refreshJsonSchema();
    });
  }

  ngOnDestroy() {
    if (this.listener) {
      this.listener.unsubscribe();
    }
  }

  refreshJsonSchema() {
    var properties: any = {
      id: {
        type: 'string',
        example: 'A234-1234-1234'
      },
      source: {
        type: 'string',
        example: 'urn:com.asyncapi.examples.user'
      },
      type: {
        type: 'string',
        example: 'com.github.pull.create'
      }
    };
    let result : any = {
      $schema: "http://json-schema.org/schema",
      type: "object",
      properties: properties
    };
    let messageJsonSchema : any = {};
    const messageJsonSchemaStr = this.addMessageDefFormGroup.get('messageJsonSchema')?.value;
    if (messageJsonSchemaStr) {
      try {
        messageJsonSchema = JSON.parse(messageJsonSchemaStr);
      }
      catch {

      }
    }

    if (messageJsonSchema.properties) {
      for (var propertyKey in messageJsonSchema.properties) {
        if (this.cloudEvtProperties.includes(propertyKey)) {
          continue;
        }

        result.properties[propertyKey] = messageJsonSchema.properties[propertyKey];
      }
    }

    if (messageJsonSchema.required) {
      result.required = messageJsonSchema.required;
    }

    this.addMessageDefFormGroup.get('jsonSchema')?.setValue(JSON.stringify(result, null, "\t"));
  }

  private extractJsonMessage(jsonSchemaStr: string) {
    var obj: any = {};
    if (jsonSchemaStr) {
      var properties: any = {};
      var jsonSchema = JSON.parse(jsonSchemaStr);
      if (jsonSchema.properties) {
        for (var propertyKey in jsonSchema.properties) {
          if (this.cloudEvtProperties.includes(propertyKey)) {
            continue;
          }

          properties[propertyKey] = jsonSchema.properties[propertyKey];
        }

        if (Object.keys(properties).length > 0) {
          obj.properties = properties;
        }

        if (jsonSchema.required) {
          obj.required = jsonSchema.required;
        }
      }
    }

    return JSON.stringify(obj, null, "\t");
  }
}
