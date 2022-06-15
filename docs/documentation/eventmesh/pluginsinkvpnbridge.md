# VPN Bridge sink

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
| eventMeshUrl      	| EventMesh server URL                       	| localhost     	|
| eventMeshPort     	| EventMesh server Port                      	| 4000         	    |
| eventMeshVpn      	| EventMesh server VPN                       	| default       	|
| clientId      		| Client identifier used to publish message  	| publishClientId   |

TODO