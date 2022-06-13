# AMQP 1.0 protocol plugin

**Name**

ProtocolAmqp

**Description**

AMQP 1.0 protocol.

**Link**

The ZIP file can be downloaded [here]().

**Options**

| Name              | Description                                                                                 | Default value |
| ----------------- | ------------------------------------------------------------------------------------------- | ------------- |
| port              | AMQP Port                                                                                   | 5672          |
| maxFrameSize      | Largest frame size that the sending peer is able to accept on this connection.              | 1000          |
| maxChannel        | The channel-max value is the highest channel number that can be used on the connection      | 1000          |
| sessionLinkCredit | Current maximum number of messages that can be handled at the receiver endpoint of the link | 255           |
| eventMeshVpn      | EventMesh server VPN                                                                        | default       |
| eventMeshUrl      | EventMesh server URL                                                                        | localhost     |
| eventMeshPort     | EventMesh server Port                                                                       | 4000          |

## Quick start

TODO