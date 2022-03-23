import { ApplicationResult } from "@stores/vpn/models/application.model";
import { ApplicationLinkResult } from "@stores/vpn/models/applicationlink.model";

export class AsyncApiBuilder {
  public static build(rootTopic: string, app: ApplicationResult, consumedLinks: ApplicationLinkResult[]) {
    var result: any = {
      "asyncapi": "2.2.0",
      "info" : this.buildInfo(app)
    };

    var channels: any = {};
    this.buildChannels(channels, rootTopic, app.links, false);
    this.buildChannels(channels, rootTopic, consumedLinks, true);
    let keys = Object.keys(channels);
    if (keys.length > 0) {
      result["channels"] = channels;
    }

    let messages: any = {};
    this.buildMessages(messages, app.links);
    this.buildMessages(messages, consumedLinks);
    keys = Object.keys(messages);
    if (keys.length > 0) {
      result["components"] = {
        "messages": messages
      };
    }
    return result;
  }

  private static buildInfo(app: ApplicationResult) {
    let info: any = {
      "version": "1.0.0"
    };
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

  private static buildMessages(result: any, links: ApplicationLinkResult[]) {
    links.forEach((l) => {
      if (l.message) {
        var keys = Object.keys(result);
        if (!keys.includes(l.message.name)) {
          result[l.message.name] = {
            payload: JSON.parse(l.message.jsonSchema)
          }
        }
      }
    });
  }

  private static buildChannels(result: any, rootTopic: string, links: ApplicationLinkResult[], isSubscribe: boolean) {
    links.forEach((l) => {
      if (!l.message) {
        return;
      }

      const topic = rootTopic + "/" + l.topicName;
      const key = isSubscribe ? "subscribe" : "publish";
      result[topic] = {};
      result[topic][key] = {
        message: {
          "$ref": "#/components/messages/" + l.message?.name
        }
      };
    });
  }
}
