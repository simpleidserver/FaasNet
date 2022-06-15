# Kafka sink

**Name**

SinkKafka

**Description**

Consume events from Kafka server.

**Link**

The ZIP file can be downloaded [here]().

**Options**

| Name                  | Description                           	 | Default value 	 |
| --------------------- | ------------------------------------------ | ----------------- |
| jobId      			| Job identifier              				 | EventMeshKafka    |
| kafkaBootstrapServers | List of brokers      						 | localhost:29092	 |
| kafkaGroupId 			| Client group identifier 					 | EventMeshKafka    |
| kafkaMetadataTimeout  | Timeout in MS to fetch metadata from Kafka | 2 seconds         |
| kafkaTopicTimerMS     | Timer in MS used to fetch kafka topics     | 5 seconds         |
| eventMeshUrl      	| EventMesh server URL                       | localhost     	 |
| eventMeshPort     	| EventMesh server Port                      | 4000         	 |
| eventMeshVpn      	| EventMesh server VPN                       | default       	 |
| clientId      		| Client identifier used to publish message  | publishClientId   |

## Quick start

Once the plugin `SinkKafka` is configured and enabled, you can deploy an Apache Kafka server.

In this tutorial we will explain how to deploy Apache Kafka by using Docker.

### Deploy Apache Kafka via Docker

Download the file

```
https://raw.githubusercontent.com/simpleidserver/FaasNet/master/Samples/FaasNet.EventMesh.KafkaSink/docker-compose.yml
```

Open a command prompt and execute the command below to deploy Apache Kafka

```
docker-compose up
```

### Update plugin configuration

Use the client identifier `publishClient`.

```
FaasNet.EventMeshCTL.CLI.exe update_plugin_configuration --name=SinkKafka --key=clientId --value=publishClient
```

### Enable the plugin

Enable the plugin and restart the EventMesh server to take into account your changes.

```
FaasNet.EventMeshCTL.CLI.exe enable_plugin --name=SinkKafka
```

Now the EventMesh server is running, it should be able to capture all the events coming from Apache Kafka.
You can publish some messages in Apache Kafka and check if they are captured by the EventMesh server.