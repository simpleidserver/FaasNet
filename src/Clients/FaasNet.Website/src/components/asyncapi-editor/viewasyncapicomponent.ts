import { Component } from "@angular/core";
import { ApplicationResult } from "@stores/applicationdomains/models/application.model";
import { ApplicationLinkResult } from "@stores/applicationdomains/models/applicationlink.model";
import { MessageDefinitionResult } from "@stores/messagedefinitions/models/messagedefinition.model";
import { MatPanelContent } from "../matpanel/matpanelcontent";
import { AsyncApiBuilder } from "./builders/asyncapibuilder";
import { Document } from 'yaml';

export class ViewAsyncApiData {
  rootTopic: string = "";
  application: ApplicationResult | null = null;
  consumedLinks: ApplicationLinkResult[] = [];
  messages: MessageDefinitionResult[] = [];
}

@Component({
  selector: 'viewasyncapi',
  templateUrl: './viewasyncapi.component.html',
  styleUrls: ['./viewasyncapi.component.scss']
})
export class ViewAsyncApiComponent extends MatPanelContent {
  jsonOptions = { theme: 'vs', language: 'json', automaticLayout: true };
  yamlOptions = { theme: 'vs', language: 'yaml', automaticLayout: true };
  yaml: string = "";
  json: string = "";

  constructor() {
    super();
  }

  override init(data: any) {
    var viewAsyncApiData = data as ViewAsyncApiData;
    if (!viewAsyncApiData || !viewAsyncApiData.application) {
      return;
    }

    var json = AsyncApiBuilder.build(viewAsyncApiData.rootTopic, viewAsyncApiData.application, viewAsyncApiData.consumedLinks, viewAsyncApiData.messages);
    this.json = JSON.stringify(json, null, "\t");
    const doc = new Document();
    doc.contents = json;
    this.yaml = doc.toString();
  }
}
