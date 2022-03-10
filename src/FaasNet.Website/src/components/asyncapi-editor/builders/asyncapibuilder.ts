import { Application } from "../models/application.model";
import { ApplicationLink } from "../models/link.model";

export class AsyncApiBuilder {
  public static build(app: Application, consumedLinks: ApplicationLink[]) {
    const messages = this.buildMessages(app, consumedLinks);
    // const components: any = {
    //   "messages": messages
    // };
    // result["components"] = components;

    var result: any = {
      "asyncapi": "2.2.0",
      "info" : this.buildInfo(app)
    };
    return result;
  }

  private static buildInfo(app: Application) {
    let info: any = {};
    if (app.title) {
      info["title"] = app.title;
    }

    if (app.version) {
      info["version"] = app.version;
    }

    if (app.description) {
      info["description"] = app.description;
    }

    return info;
  }

  private static buildComponents(app: Application, consumedLinked: ApplicationLink[]) {

  }

  private static buildMessages(app: Application, consumedLinks: ApplicationLink[]): any {
    var result: any = {};
    app.links.forEach((l) => {
      l.evts.forEach((e) => {
        var keys = Object.keys(result);
        if (!keys.includes(e.name)) {
          result[e.name] = e.payload;
        }
      });
    });

    consumedLinks.forEach((cl) => {
      cl.evts.forEach((e) => {
        var keys = Object.keys(result);
        if (!keys.includes(e.name)) {
          result[e.name] = e.payload;
        }
      });
    });

    return result;
  }

  private static buildChannels(app: Application, consumedLinks: ApplicationLink[]): any {
    var result = {

    };
    return result;
  }
}
