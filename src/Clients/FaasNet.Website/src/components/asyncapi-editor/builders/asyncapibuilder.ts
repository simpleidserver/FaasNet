import { ApplicationResult } from "@stores/applicationdomains/models/application.model";
import { ApplicationLinkResult } from "@stores/applicationdomains/models/applicationlink.model";
import { MessageDefinitionResult } from "../../../stores/messagedefinitions/models/messagedefinition.model";

export class AsyncApiBuilder {
  public static build(rootTopic: string, app: ApplicationResult, consumedLinks: ApplicationLinkResult[], msgs: MessageDefinitionResult[]) {
    var result: any = {
      "asyncapi": "2.2.0",
      "info" : this.buildInfo(app)
    };

    var channels: any = {};
    this.buildChannels(channels, rootTopic, app.links, false, msgs);
    this.buildChannels(channels, rootTopic, consumedLinks, true, msgs);
    let keys = Object.keys(channels);
    if (keys.length > 0) {
      result["channels"] = channels;
    }

    let messages: any = {};
    this.buildMessages(messages, app.links, msgs);
    this.buildMessages(messages, consumedLinks, msgs);
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
      "version": "0.0.1"
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

  private static buildMessages(result: any, links: ApplicationLinkResult[], messages: MessageDefinitionResult[]) {
    links.forEach((l) => {
      if (!l.messageId) {
        return;
      }

      const message = messages.filter((m) => m.id === l.messageId)[0];
      if (message) {
        var keys = Object.keys(result);
        if (!keys.includes(message.name)) {
          result[message.name] = {
            payload: JSON.parse(message.jsonSchema)
          }
        }
      }
    });
  }

  private static buildChannels(result: any, rootTopic: string, links: ApplicationLinkResult[], isSubscribe: boolean, messages: MessageDefinitionResult[]) {
    links.forEach((l) => {
      if (!l.messageId) {
        return;
      }

      const message = messages.filter((m) => m.id === l.messageId)[0];
      const topic = rootTopic + "/" + l.topicName;
      const key = isSubscribe ? "subscribe" : "publish";
      result[topic] = {};
      result[topic][key] = {
        message: {
          "$ref": "#/components/messages/" + message.name
        }
      };
    });
  }
}
