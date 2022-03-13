import { Component } from "@angular/core";
import { MatPanelContent } from "../matpanel/matpanelcontent";
import { AsyncApiBuilder } from "./builders/asyncapibuilder";
import { Application } from "./models/application.model";
import { ApplicationLink } from "./models/link.model";

export class ViewAsyncApiData {
  application: Application | null = null;
  consumedLinks: ApplicationLink[] = [];
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

    var json = AsyncApiBuilder.build(viewAsyncApiData.application, viewAsyncApiData.consumedLinks);
    this.json = JSON.stringify(json, null, "\t");
  }
}
