import { Application } from "../models/application.model";
import { ApplicationLink } from "../models/link.model";

export class AsyncApiBuilder {
  public static build(app: Application, consumedLinks: ApplicationLink[]) {
    const messages = this.buildMessages(app, consumedLinks);
    const components: any = {
      "messages": messages
    };
    var result = {
      "title": app.title,
      "version": app.version,
      "description": app.description,
      "components": components
    };
    return result;
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
