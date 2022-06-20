# AMQP 1.0 sink

**Name**

SinkAMQP

**Description**

Consume events from AMQP 1.0 server.

**Link**

The ZIP file can be downloaded [here]().

**Options**

| Name              | Description                           	| Default value 		|
| ----------------- | ----------------------------------------- | --------------------- |
| jobId      		| Job identifier              				| AMQPTopic     	    |
| amqpHost        	| AMQP server host      					| 127.0.0.1		        |
| amqpPort 			| AMQP server port 							| 5672           		|
| amqpTopicName     | Name of the topic exchange                | amq.topic          	|
| amqpUserName      | AMQP username                             | guest          		|
| amqpPassword      | AMQP password                             | guest          		|
| eventMeshUrl      | EventMesh peer URL                     	| localhost     		|
| eventMeshPort     | EventMesh peer Port                    	| 4000         			|
| eventMeshVpn      | EventMesh peer VPN                     	| default       		|
| clientId      	| Client identifier used to publish message | publishClientId       |

## Quick start

Once you have an up and running EventMesh peer with `SinkAMQP` plugin installed, you can deploy an AMQP 1.0 server such as RabbitMQ.

In this tutorial we will explain how to deploy a local RabbitMQ server with the plugin `rabbitmq_amqp1_0` enabled via Docker.

### Configure client and VPN

Before going further, a Virtual Private Network (VPN) and one client must be configured.
Those information will be used to publish message.

Open a command prompt and create a topic named `default` :

```
FaasNet.EventMeshCTL.CLI.exe add_vpn --name=default
```

Add a client `publishClientId`, as the name suggests, it will be used to publish message.

```
FaasNet.EventMeshCTL.CLI.exe add_client --vpn=default --identifier=publishClientId --publish_enabled=true --subscription_enabled=false
```

### Deploy RabbitMQ via Docker

Download the Docker file

```
https://raw.githubusercontent.com/simpleidserver/FaasNet/master/Samples/FaasNet.EventMesh.AmqpSink/AmqpServerDockerFile
```

Open a command prompt and build the docker image

```
docker build -t amqpserver -f AmqpServerDockerFile .
```

Run RabbitMQ peer with the plugin `rabbitmq_amqp1_0` enabled.

```
docker run --name rabbitmq -p 5672:5672 -p 15672:15672 amqpserver
```

### Enable the plugin

Enable the plugin and restart the EventMesh peer to take into account your changes.

By default, the configuration is correct and there is no need to update it.

```
FaasNet.EventMeshCTL.CLI.exe enable_plugin --name=SinkAMQP
```

Now the EventMesh peer is running, it should be able to capture all the events coming from the exchange name `amq.topic`.
You can publish some messages in RabbitMQ and check if they are captured by the EventMesh peer.