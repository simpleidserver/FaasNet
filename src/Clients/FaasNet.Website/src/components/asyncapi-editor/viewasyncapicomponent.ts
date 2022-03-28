import { Component } from "@angular/core";
import { ApplicationResult } from "@stores/applicationdomains/models/application.model";
import { ApplicationLinkResult } from "@stores/applicationdomains/models/applicationlink.model";
import { MatPanelContent } from "../matpanel/matpanelcontent";
import { AsyncApiBuilder } from "./builders/asyncapibuilder";

export class ViewAsyncApiData {
  rootTopic: string = "";
  application: ApplicationResult | null = null;
  consumedLinks: ApplicationLinkResult[] = [];
}

@Component({
  selector: 'viewasyncapi',
  templateUrl: './viewasyncapi.component.html',
  styleUrls: ['./viewasyncapi.component.scss']
})
export class ViewAsyncApiComponent extends MatPanelContent {
  jsonOptions = { theme: 'vs', language: 'json', automaticLayout: true };
  json: string = "";

  constructor() {
    super();
  }

  override init(data: any) {
    var viewAsyncApiData = data as ViewAsyncApiData;
    if (!viewAsyncApiData || !viewAsyncApiData.application) {
      return;
    }

    var json = AsyncApiBuilder.build(viewAsyncApiData.rootTopic, viewAsyncApiData.application, viewAsyncApiData.consumedLinks);
    this.json = JSON.stringify(json, null, "\t");
  }
}
