# RabbitMQ

An EventMesh server can be configured to consume and publish messages from / to RabbitMQ.

> [!WARNING]
> Before you start, Make sure there is a Visual Studio Solution with a [configured EventMesh server](/documentation/eventmesh/installation.html).

## Source Code

The source code of this project can be found [here](https://github.com/simpleidserver/FaasNet/tree/master/samples/EventMeshServerRabbitMQ).

## Configure EventMesh server

The Nuget package `EventMesh.Runtime.AMQP` must be installed.

```
dotnet add package EventMesh.Runtime.AMQP
```

Edit the file which contains the configuration of the EventMesh server and add the line `AddAMQP()` after `RuntimeHostBuilder` or `AddRuntime`.

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

The `AddAMQP` accepts one optional parameter in entry. It can be used to update update the options.

