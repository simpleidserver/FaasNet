# Websocket protocol plugin

**Name**

ProtocolWebsocket

**Description**

Support Websocket protocol.

**Link**

The ZIP file can be downloaded [here]().

**Options**

| Name              | Description                                                                                 | Default value |
| ----------------- | ------------------------------------------------------------------------------------------- | ------------- |
| port              | Websocket port                                                                              | 2803          |
| eventMeshUrl      | EventMesh server URL                                                                        | localhost     |
| eventMeshPort     | EventMesh server Port                                                                       | 4000          |

## Quick start

Once you have an up and running EventMesh server with `ProtocolWebsocket` plugin enabled, you can start using any client compliant with the [WebSocket protocol](https://datatracker.ietf.org/doc/html/rfc6455).

### Configure client and VPN

Before going further, a Virtual Private Network (VPN) and two clients must be configured.
Those information will be used to publish message and subscribe to one topic.

Open a command prompt and create a topic named `default` :

```
FaasNet.EventMeshCTL.CLI.exe add_vpn --name=default
```

Add a client `publishClient`, as the name suggests, it will be used to publish message.

```
FaasNet.EventMeshCTL.CLI.exe add_client --vpn=default --identifier=publishClient --publish_enabled=true --subscription_enabled=false
```

Add a client `subscribeClient`, this client will be used for subscription.

```
FaasNet.EventMeshCTL.CLI.exe add_client --vpn=default --identifier=subscribeClient --publish_enabled=false --subscription_enabled=true
```

### Configure the plugin

If the plugin is not yet configured, it can be enabled like this

```
FaasNet.EventMeshCTL.CLI.exe enable_plugin --name=ProtocolWebsocket
```

Its configuration can be updated either by [using CLI](cli.md) or by updating the configuration file `appsettings.json`.

Don't forget that the EventMesh server must be restarted, otherwise the changes are not taken into account.

When the configuration is finished, a client can be created and can start publishing message.

## Source code

The source code of this project can be found [here]().

## Create a client

In this tutorial, we will explain how to create a javascript client to publish message and subscribe to one topic.

Create an HTML page and add the following content :

```
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
	<style>
		#messages {
			list-style-type: none;
			padding: 0px;
			margin: 0px;
		}
		
		#messages > li {
			padding: 5px;
			border: 1px solid gray;
		}
	</style>
</head>
<body>
	<div>
		<h1>Publish</h1>
		<div>
			<div>
				<label>Content</label>
				<input type="text" id="messageTxt" />
			</div>
			<div>
				<label>Topic</label>
				<input type="text" id="messageTopic" />
			</div>
			<button id="sendMessage">Send</button>
		</div>
	</div>
	<div>
		<h1>Subscribe</h1>
		<div id="sub">
			<div>
				<label>Topic</label>
				<input type="text" id="subTopic" />
			</div>
			<button id="startSubscription">Start subscription</button>
		</div>
		<ul id="messages">
			
		</ul>
	</div>
	<script src="https://code.jquery.com/jquery-3.6.0.js" integrity="sha256-H+K7U5CnXl1h5ywQfKtSj8PCmoN9aaq30gDh27Xc0jk=" crossorigin="anonymous"></script>
	<script>
		const ws = new WebSocket("ws://localhost:2803");
		$(document).ready(function() {
			var initSubscription = function() {
				ws.onmessage = function(data) {
					$("#messages").append("<li>" + data.data + "</li>");
				};
				$("#startSubscription").click(function() {
					const subTopic = $("#subTopic").val();
					var directSubscribe = {
						requestType: "DIRECTLY_SUBSCRIBE",
						vpn: "default",
						clientId: "subscribeClient",
						filter: subTopic
					};
					const json = JSON.stringify(directSubscribe);
					ws.send(json);
					$("#sub").hide();
				});
			};
			var initPublish = function() {
				$("#sendMessage").click(function() {
					const msg = $("#messageTxt").val();
					const topic = $("#messageTopic").val();
					var publishMsg = {
						requestType: 'PUBLISH',
						vpn: 'default',
						id: 'id',
						subject: 'subject',
						topic: topic,
						clientId: 'publishClient',
						content: msg
					};
					const json = JSON.stringify(publishMsg);
					ws.send(json);
					$("#messageTxt").val('');
					$("#messageTopic").val('');
				});
			};
			initSubscription();
			initPublish();
		});
	</script>
</body>
</html>
```

Open your preferred web browser and browse the HTML file.

There are two blocks displayed in the UI. 

Under the `Subscribe` block, enter the topic name for example `q1` and click on the button `Start subscription` to start listening messages coming from the topic `q1`.

Under the `Publish` block, enter the topic name `q1`, write a message and click on the `Send` button.

The message will be transferred to the EventMesh server and will be displayed in the HTML page.

The websocket plugin accepts two types of request. 

**Subscribe request**

| Property    | Description       | Default value      |
| ----------- | ----------------- | ------------------ |
| requestType | Type of request   | DIRECTLY_SUBSCRIBE |
| vpn         | VPN name          |                    |
| clientId    | Client identifier |                    |
| filter      |                   |                    |

**Publish request**

| Property    | Description                                                                  | Default value |
| ----------- | ---------------------------------------------------------------------------- | ------------- |
| requestType | Type of request                                                              | PUBLISH       |
| vpn         | VPN name                                                                     |               |
| id          | An identifier for the event                                                  |               |
| subject     | This describes the subject of the event in the context of the event producer |               |
| topic       |                                                                              |               |
| clientId    | Client identifier                                                            |               |
| content     | Event data specific to the event type                                        |               |