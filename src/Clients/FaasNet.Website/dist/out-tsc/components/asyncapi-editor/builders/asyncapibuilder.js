export class AsyncApiBuilder {
    static build(app, consumedLinks) {
        const messages = this.buildMessages(app, consumedLinks);
        // const components: any = {
        //   "messages": messages
        // };
        // result["components"] = components;
        var result = {
            "asyncapi": "2.2.0",
            "info": this.buildInfo(app)
        };
        return result;
    }
    static buildInfo(app) {
        let info = {};
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
    static buildComponents(app, consumedLinked) {
    }
    static buildMessages(app, consumedLinks) {
        var result = {};
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
    static buildChannels(app, consumedLinks) {
        var result = {};
        return result;
    }
}
//# sourceMappingURL=asyncapibuilder.js.map