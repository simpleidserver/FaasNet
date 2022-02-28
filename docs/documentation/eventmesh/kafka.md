# Kafka

An EventMesh server can be configured to consume and publish messages from / to Apache Kafka.

> [!WARNING]
> Before you start, Make sure there is a Visual Studio Solution with a [configured EventMesh server](/documentation/eventmesh/installation.html).

## Source Code

The source code of this project can be found [here](https://github.com/simpleidserver/FaasNet/tree/master/samples/EventMeshServerKafka).

## Configure EventMesh server

The Nuget package `EventMesh.Runtime.Kafka` must be installed.

```
dotnet add package EventMesh.Runtime.Kafka
```


Edit the file containing the configuration of the EventMesh server and add the line `AddKafka()` after `RuntimeHostBuilder` or `AddRuntime`.

Standalone EventMesh server :

```
new RuntimeHostBuilder(opt =>
{
    opt.Port = "localhost";
    opt.Urn = 4000;
}).AddKafka();
```


EventMesh server with UI :

```
 services.AddRuntimeWebsite(opt =>
{
    opt.Urn = "localhost";
    opt.Port = 4000;
}).AddKafka();
```

The function `AddKafka` accepts one optional parameter in entry. It can be used to configure the behavior of the library.
The following properties can be configured :


| Property           | Description                                                                                          | Default value           |
| ------------------ | ---------------------------------------------------------------------------------------------------- | ----------------------- |
| BootstrapServers   | Initial list of brokers as a CSV list of broker host or host:port.                                   | localhost:29092         |
| BrokerName         | Name of the configured message broker. This value must be unique for each configure message broker   | kafka                   |