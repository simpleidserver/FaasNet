# Kafka sink

**Name**

SinkKafka

**Description**

Consume events from Kafka server.

**Link**

The ZIP file can be downloaded [here](https://github.com/simpleidserver/FaasNet/releases/latest/download/EventMeshSinksKafka.zip).

**Options**

| Name                  | Description                           	 | Default value 	 |
| --------------------- | ------------------------------------------ | ----------------- |
| jobId      			| Job identifier              				 | EventMeshKafka    |
| kafkaBootstrapServers | List of brokers      						 | localhost:29092	 |
| kafkaGroupId 			| Client group identifier 					 | EventMeshKafka    |
| kafkaMetadataTimeout  | Timeout in MS to fetch metadata from Kafka | 2 seconds         |
| kafkaTopicTimerMS     | Timer in MS used to fetch kafka topics     | 5 seconds         |
| eventMeshUrl      	| EventMesh peer URL                       	 | localhost     	 |
| eventMeshPort     	| EventMesh peer Port                      	 | 4000         	 |
| eventMeshVpn      	| EventMesh peer VPN                       	 | default       	 |
| clientId      		| Client identifier used to publish message  | publishClientId   |

## Quick start

Once you have an up and running EventMesh peer with `SinkKafka` plugin installed, you can deploy an Apache Kafka server.

In this tutorial we will explain how to deploy Apache Kafka by using Docker.

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

### Deploy Apache Kafka via Docker

Download the file

```
https://raw.githubusercontent.com/simpleidserver/FaasNet/master/src/Samples/FaasNet.EventMesh.KafkaSink/docker-compose.yml
```

Open a command prompt and execute the command below to deploy Apache Kafka

```
docker-compose up
```

### Enable the plugin

Enable the plugin and restart the EventMesh peer to take into account your changes.

```
FaasNet.EventMeshCTL.CLI.exe enable_plugin --name=SinkKafka
```

Now the EventMesh peer is running, it should be able to capture all the events coming from Apache Kafka.
You can publish some messages in Apache Kafka and check if they are captured by the EventMesh peer.