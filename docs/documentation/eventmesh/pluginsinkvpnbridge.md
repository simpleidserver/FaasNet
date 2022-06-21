# Message VPN Bridge sink

**Name**

SinkVpnBridge

**Description**

Consume events from VPN bridge

**Link**

The ZIP file can be downloaded [here]().

**Options**

| Name                  | Description                           	    | Default value     |
| --------------------- | --------------------------------------------- | ----------------- |
| jobId      			| Job identifier              				 	| VpnBridge    	 	|
| bridgeTimerMS 		| Timer in MS to get bridge servers      		| 5 seconds	     	|
| bridgeGroupId 		| Group identifier used to subscribe to a topic | VpnBridgeGroupId  |
| eventMeshUrl      	| EventMesh peer URL                       	    | localhost     	|
| eventMeshPort     	| EventMesh peer Port                      	    | 4000         	    |
| eventMeshVpn      	| EventMesh peer VPN                       	    | default       	|
| clientId      		| Client identifier used to publish message  	| publishClientId   |

## Quick start

To establish a bridge between two Message VPN, we are going to use Docker to deploy two EventMesh clusters.

Download the Docker compose file

```
https://raw.githubusercontent.com/simpleidserver/FaasNet/master/src/Samples/FaasNet.EventMesh.TwoCluster/docker-compose.yml
```

Open a command prompt and execute the following command 

```
docker-compose up
```

Two EventMesh clusters will be deployed and will listen the ports `4001` and `4002`.

### Configure the client, VPN and the bridge

Add a new Message VPN `default` in both clusters.

```
FaasNet.EventMeshCTL.CLI.exe add_vpn --name=default --port=4001
FaasNet.EventMeshCTL.CLI.exe add_vpn --name=default --port=4002
```

Add a client `publishClient` in the cluster `4002`. It will be used by the CLI to publish message

```
FaasNet.EventMeshCTL.CLI.exe add_client --vpn=default --identifier=publishClient --publish_enabled=true --subscription_enabled=false --port=4002
```

Add a client `subscribeClient` in the cluster `4002`. It will be used by the cluster `4001` to receive message.

```
FaasNet.EventMeshCTL.CLI.exe add_client --vpn=default --identifier=subscribeClient --publish_enabled=false --subscription_enabled=true --port=4002
```

Add a client `publishClientId` in the cluster `4001`. It will be used by the `SinkVpnBridge` plugin to publish message.

```
FaasNet.EventMeshCTL.CLI.exe add_client --vpn=default --identifier=publishClientId --publish_enabled=true --subscription_enabled=false --port=4001
```

Add a client `subscribeClient` in the cluster `4001`. It will be used by the CLI to receive message.

```
FaasNet.EventMeshCTL.CLI.exe add_client --vpn=default --identifier=subscribeClient --publish_enabled=false --subscription_enabled=true --port=4001
```

Add a bridge between both Message VPN

```
FaasNet.EventMeshCTL.CLI.exe add_vpn_bridge --vpn=default --tvpn=default --turn=evtmeshnode2 --tport=4000 --tid=subscribeClient --port=4001
```

### Configure and enable the plugin

Enable the plugin and restart the EventMesh cluster to take into account your changes.

```
FaasNet.EventMeshCTL.CLI.exe enable_plugin --name=SinkVpnBridge --port=4001
```

Now both EventMesh clusters are running, the cluster `4001` should be able to capture all the events coming from the cluster `4002`.

Publish a message with the content `hello` to the topic `topic`

```
FaasNet.EventMeshCTL.CLI.exe publish_message --vpn=default --identifier=publishClient --topic=topic --message=hello --port=4002
```

Read the first message from the topic `topic`. At the end, the message should be displayed.

```
FaasNet.EventMeshCTL.CLI.exe read_message --vpn=default --identifier=subscribeClient --topic=topic --port=4001
```