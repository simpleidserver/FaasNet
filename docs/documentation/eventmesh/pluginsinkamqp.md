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
| eventMeshUrl      | EventMesh server URL                      | localhost     		|
| eventMeshPort     | EventMesh server Port                     | 4000         			|
| eventMeshVpn      | EventMesh server VPN                      | default       		|
| clientId      	| Client identifier used to publish message | publishClientId       |

## Quick start

Once the plugin `SinkAMQP` is configured and enabled, you can deploy an AMQP 1.0 server such as RabbitMQ.

In this tutorial we will explain how to deploy a local RabbitMQ server with the plugin `rabbitmq_amqp1_0` enabled via Kubernetes.

### Deploy RabbitMQ via Kubernetes

Open a command prompt and install the RabbitMQ Cluster Kubernetes Operator

```
kubectl apply -f "https://github.com/rabbitmq/cluster-operator/releases/latest/download/cluster-operator.yml"
```

Deploy RabbitMQ

```
kubectl apply -f "https://raw.githubusercontent.com/simpleidserver/FaasNet/master/src/Samples/FaasNet.EventMesh.AmqpSink/rabbitmq.yml"
```

When the RabbitMQ server is running, fetch the default credentials username/password from the Kubernetes secrets.
Both values are encoded in base64 and must be decoded.

```
kubectl get secret hello-world-default-user -o jsonpath='{.data.user}'
kubectl get secret hello-world-default-user -o jsonpath='{.data.password}'
```

### Update plugin configuration

Always in a command prompt, use the CLI to update the username and password with the values obtained from the previous step.

```
FaasNet.EventMeshCTL.CLI.exe update_plugin_configuration --name=SinkAMQP --key=amqpUserName --value=<USERNAME>
FaasNet.EventMeshCTL.CLI.exe update_plugin_configuration --name=SinkAMQP --key=amqpPassword --value=<PASSWORD>
```

Use the AMQP port `30007`.

```
FaasNet.EventMeshCTL.CLI.exe update_plugin_configuration --name=SinkAMQP --key=amqpPort --value=30007
```

And use the client identifier `publishClient`.

```
FaasNet.EventMeshCTL.CLI.exe update_plugin_configuration --name=SinkAMQP --key=clientId --value=publishClient
```

### Enable the plugin

Enable the plugin and restart the EventMesh server to take into account your changes.

```
FaasNet.EventMeshCTL.CLI.exe enable_plugin --name=SinkAMQP
```

Now the EventMesh server is running, it should be able to capture all the events coming from the exchange name `amq.topic`.
You can publish some messages in RabbitMQ and check if they are captured by the EventMesh server.