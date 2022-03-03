# RabbitMQ

An EventMesh server can be configured to consume and publish messages from / to RabbitMQ.

> [!WARNING]
> Before you start, Make sure there is a Visual Studio Solution with a [configured EventMesh server](/documentation/eventmesh/installation.html).

## Source Code

The source code of this project can be found [here](https://github.com/simpleidserver/FaasNet/tree/master/samples/EventMeshServerRabbitMQ).

## Configure EventMesh server

The Nuget package `FaasNet.EventMesh.Runtime.AMQP` must be installed.

```
dotnet add package FaasNet.EventMesh.Runtime.AMQP
```

Edit the file containing the configuration of the EventMesh server and add the line `AddAMQP()` after `RuntimeHostBuilder` or `AddRuntime`.

Standalone EventMesh server :

```
new RuntimeHostBuilder(opt =>
{
    opt.Port = "localhost";
    opt.Urn = 4000;
}).AddAMQP();
```


EventMesh server with UI :

```
 services.AddRuntimeWebsite(opt =>
{
    opt.Urn = "localhost";
    opt.Port = 4000;
}).AddAMQP();
```

The function `AddAMQP` accepts one optional parameter in entry. It can be used to configure the behavior of the library.
The following properties can be configured :

| Property           | Description                                                                                                             | Default value                                                             |
| ------------------ | ----------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------- |
| TopicName          | Name of the RabbitMQ exchange. Used during publish-subscribe                                                            | amq.topic                                                                 |
| QueueName          | A queue with the following pattern `{QueueName}-{TopicName}` is created when a session subscribe to a topic             | streamQueue                                                               |
| BrokerName         | Name of the configured message broker. This value must be unique for each configure message broker                      | amqp                                                                      |
| ConnectionFactory  | Contains the configuration used to establish connection with RabbitMQ                                                   | HostName: 127.0.0.1, Port: 5672, UserName: guest, Password: guest         |