# EventMesh protocol

## Concepts

Session
* A session is needed to send commands to the server like: Subscribe to a topic.

Bridge
* A bridge can be created between two servers. Subscription to one or more topics happened to all the servers present in the configured network.

EventMesh server
* An EventMesh server is linked to one or more message brokers (RabbitMQ / Apache Kafka etc...)
* All the messages are translated into CloudEvents.

## Common Structure

### Message structure

| Field size | Description        | Data Type | Comments                                                                      |
| ---------- | ------------------ | --------- | ----------------------------------------------------------------------------- |
| 10		 | Magic Value        | string    | Magic value, must be equal to  "EventMesh"                                    |
| 5          | Protocol Version   | string    | Protocol version number, must be equal to 0000                                |
| 4          | Command            | int32  	  | Identification of the command                                                 |
| 4          | Status Code        | int32     | Identification of the status                                                  |
| ANY        | Status Description | string    | Short description of the status                                               |
| 11         | Seq                | string    | Correlation number of the request. Response must have the same seq number     |
| any        | body               | object    | Content of the command                                                        |

Known commands:

| Code | Name                        |
| ---- | --------------------------- |
| 0    | HEARBEAT_REQUEST            |
| 1    | HEARTBEAT_RESPONSE          |
| 2    | HELLO_REQUEST               |
| 3    | HELLO_RESPONSE              |
| 4    | SUBSCRIBE_REQUEST           |
| 5    | SUBSCRIBE_RESPONSE          |
| 6    | ASYNC_MESSAGE_TO_CLIENT     |
| 7    | ASYNC_MESSAGE_TO_CLIENT_ACK |
| 8    | ADD_BRIDGE_REQUEST          |
| 9    | ADD_BRIDGE_RESPONSE         |
| 11   | DISCONNECT_REQUEST          |
| 12   | DISCONNECT_RESPONSE         |

Known status:

| Code | Description |
| ---- | ----------- |
| 0    | success     |
| 1    | fail        |
| 2    | aclFail     |
| 3    | tpsOverload |

## Commands

### Heartbeat

Hearbeat requests are sent to check the availablity of the EventMesh Runtime server.

### Hello

Hello requests are sent to create and start to communicate with the remote server.

### Subscribe

This command is used by the client to subscribe to one or more topics.

### Async Message To Client

When a message is received from a message broker then the EventMesh Runtime server sent an AsyncMessageToClient to the client.

### AddBridgeRequest

This command is used to create a bridge between two servers.

### Disconnect

Close the active session.