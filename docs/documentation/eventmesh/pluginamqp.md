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

Once you have an up and running EventMesh server with `ProtocolAmqp` plugin enabled, you can start using any client compliant with the [AMQP 1.0 protocol](http://docs.oasis-open.org/amqp/core/v1.0/os/amqp-core-complete-v1.0-os.pdf).

### Configure client and VPN

Before going further, a Virtual Private Network (VPN) and two clients must be configured.
Those information will be used to publish message and subscribe to one topic.

Open a command prompt and create a topic named `default` :

```
FaasNet.EventMeshCTL.CLI.exe add_vpn --name=default
```

Add a client `publishClient`, as the name suggests, it will be used to publish message.

```
FaasNet.EventMeshCTL.CLI.exe add_client --vpn=default --identifier=publishClient --publish_enabled=true --subscription_enabled=false
```

Add a client `subscribeClient`, this client will be used for subscription.

```
FaasNet.EventMeshCTL.CLI.exe add_client --vpn=default --identifier=subscribeClient --publish_enabled=false --subscription_enabled=true
```

### Configure the plugin

If the plugin is not yet configured, it can be enabled like this

```
FaasNet.EventMeshCTL.CLI.exe enable_plugin --name=ProtocolAmqp
```

Its configuration can be updated either by [using CLI](cli.md) or by updating the configuration file `appsettings.json`.

Don't forget that the EventMesh server must be restarted, otherwise the changes are not taken into account.

When the configuration is finished, a client can be created and can start publishing message.

## Source Code

The source code of this project can be found [here]().

## Create a client

In this tutorial, we will explain how to create a C# client to publish message and subscribe to one topic `q1`.

Create a new console application and install the Nuget package `AmqpNetLite`

```
dotnet add package AmqpNetLite 
```

In the `Program.cs` file add a static method used to send message.

```
static void SendMessage()
{
    var address = new Address($"amqp://publishClient:publishClient@localhost:{port}");
    var connection = new Connection(address);
    var session = new Session(connection);
    var message = new Message("Hello AMQP!");
    var sender = new SenderLink(session, "sender-link", "q1");
    Console.WriteLine("Sent Hello AMQP!");
    sender.Send(message);
    sender.Close();
    session.Close();
    connection.Close();
}
```

Add a second static method used to subscribe to a topic :

```
static void ReceiveMessage()
{
    Task.Run(() =>
    {
        var address = new Address($"amqp://subscribeClient:subscribeClient@localhost:{port}");
        var connection = new Connection(address);
        var session = new Session(connection);
        var receiver = new ReceiverLink(session, "receiver-link", "q1");
        while(true)
        {
            var message = receiver.Receive();
            Console.WriteLine("Received " + message.Body.ToString());
        }
    });
}
```

Call both methods like this :

```
const int port = 5672;
Console.WriteLine("Hello, World!");
ReceiveMessage();
SendMessage();
Console.WriteLine("Press enter to quit the application");
Console.ReadLine();
```

Build the project and run.

The console application must display two messages :

```
Send Hello AMQP!
Received Hello AMQP!
```