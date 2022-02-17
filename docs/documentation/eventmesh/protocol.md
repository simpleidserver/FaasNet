# EventMesh protocol

## Concepts

### EventMesh Server

Acts as a proxy between message brokers and clients.

### Client

Client can be any types of application which implements the EventMesh protocol.

### Session

Client can have one or more sessions. 
There are two types of session :
* SUB : subscribe to one or more topics.
* PUB : publish messages.

### Bridge

Link between two servers.
When a bridge is established, messages can transit from one server to an another.

### CloudEvent

All messages coming from Brokers are translated into CloudEvent and vice versa.

## Common Structure

### Message structure

| Description        | Data Type | Comment                                                                       | Default Value |
| ------------------ | --------- | ----------------------------------------------------------------------------- | ------------- |
| Magic Value        | string    | Magic value. 								                                 | EventMesh     |
| Protocol Version   | string    | Protocol version.                                                             | 0000          |
| Command            | int32  	 | Identifier of the command.                                                    |               |
| Status             | int32     | Identifier of the status.                                                     |               |
| Status description | string    | Short description of the status.                                              |               |
| Error              | string    | Short description of the error .                                              |               |
| Seq                | string    | Correlation number. Request and response must have the same seq number.       |               |
| Body               | ANY       | Contains the properties of the command.                                       |               |

Known commands:

| Code | Name                                 |
| ---- | ------------------------------------ |
| 0    | HEARBEAT_REQUEST                     |
| 1    | HEARTBEAT_RESPONSE                   |
| 2    | HELLO_REQUEST                        |
| 3    | HELLO_RESPONSE                       |
| 4    | SUBSCRIBE_REQUEST                    |
| 5    | SUBSCRIBE_RESPONSE                   |
| 6    | ASYNC_MESSAGE_TO_CLIENT              |
| 7    | ASYNC_MESSAGE_TO_CLIENT_ACK          |
| 8    | ADD_BRIDGE_REQUEST                   |
| 9    | ADD_BRIDGE_RESPONSE                  |
| 10   | ASYNC_MESSAGE_TO_SERVER              |
| 11   | DISCONNECT_REQUEST                   |
| 12   | DISCONNECT_RESPONSE                  |
| 13   | ASYNC_MESSAGE_TO_CLIENT_ACK_RESPONSE |
| 14   | PUBLISH_MESSAGE_REQUEST              |
| 15   | PUBLISH_MESSAGE_RESPONSE             |

Known status:

| Code | Description |
| ---- | ----------- |
| 0    | success     |
| 1    | fail        |
| 2    | aclFail     |
| 3    | tpsOverload |

## Commands

### Heartbeat request

*Request* : HEARBEAT_REQUEST.
*Response* : HEARTBEAT_RESPONSE.

Heartbeat request are sent by client to check the availablity of an EventMesh Server.

### Hello request

*Request* : HELLO_REQUEST.
*Response* : HELLO_RESPONSE.

Client send hello request to EventMesh server to create a session.
The following informations are passed in the request. They are used by the EventMesh server to check if the client is authorized to subscribe or publish.

| Description       | Data Type | Comment                                                                                                                       |
| ----------------- | --------- | ----------------------------------------------------------------------------------------------------------------------------- |
| ClientId          | string    | Identifier of the client                                                                                                      |
| Environment       | string    | Environment used by the client. Possible values can be TST,VAL or PRD                                                         |
| Urn               | string    | URN of the EventMesh server calling an another EventMesh server.                                                              |
| Port              | int32     | Port of the EventMesh server calling an another EventMesh server.                                                             |
| Password          | string    | Password of the client. Used during the authentication phase.                                                                 |
| BufferCloudEvents | int32     | Number of messages sent to the client. Used to send a batch of messages.                                                      |
| Purpose           | int32     | Type of session. Possible values are : SUBSCRIBE = 0, PUBLISH = 1                                                             |
| IsServer          | boolean   | Indicate if the request is coming from a client or a server.                                                                  |
| Pid               | int32     | Identifier of the process.                                                                                                    |

> [!WARNING]
> When a bridge is created between two EventMesh servers. The parameters `Urn` and `Port` will be used to transmit messages from one server to the second and to the client.

When a session is created, an hello response is sent by the EventMesh server to the client.
It contains a unique session identifier. This value will be used by the client to perform future operations.

| Description | Data Type |                                  |
| ----------- | --------- | -------------------------------- |
| SessionId   | string    | Unique identifier of the session |

### Subscribe request

*Request* : SUBSCRIBE_REQUEST.
*Response* : SUBSCRIBE_RESPONSE.

> [!WARNING]
> A session is required to perform a subscription.

Client send subscribe request to EventMesh sever to subscribe to one or more topics.
The following informations are passed in the request.

| Description | Data Type | Comment                            |
| ----------- | --------- | ---------------------------------- |
| ClientId    | string    | Identifier of the client.          |
| SessionId   | string    | Unique identifier of the session.  |
| Topics      | string[]  | List of topics.                    |

### Add Bridge request

TODO

### Disconnect request

TODO

### Publish Message request

TODO