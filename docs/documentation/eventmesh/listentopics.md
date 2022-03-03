# Listen topics

> [!NOTE]
> The source code of this project can be found [here](https://github.com/simpleidserver/FaasNet/tree/master/samples/EventMeshClientSubscribe).

A client can subscribe to one or more topics and receive messages from them.

Topic subscription can be implemented by any types of DOTNET CORE application.

## Configuration steps

First, the Nuget package `FaasNet.EventMesh.Runtime` must be installed.

```
dotnet add package FaasNet.EventMesh.Runtime
```

A session must be established between the client and the server in order to subscribe to one or more topics.

```
private static async Task<string> CreateSession(RuntimeClient runtimeClient)
{
    var helloResponse = await runtimeClient.Hello(new UserAgent
    {
        ClientId = "clientId",
        Environment = "TST",
        Password = "password",
        Pid = 2000,
        BufferCloudEvents = 1,
        Version = "0",
        Purpose = UserAgentPurpose.SUB
    });
    return helloResponse.SessionId;
}
```

> [!WARNING]
> In-memory implementation of the MessageBroker doesn't support wildcard in topic name.

Subscribe to one topic `Person.Created` and display the messages received.

```
private static async Task<SubscriptionResult> SubscribeTopic(RuntimeClient runtimeClient, string sessionId)
{
    return await runtimeClient.Subscribe("clientId", sessionId, new List<SubscriptionItem>
    {
        new SubscriptionItem
        {
            Topic = "Person.Created",
        }
    }, (msg) =>
    {
        var cloudEvts = string.Join(",", msg.CloudEvents.Select(c => c.Data));
        Console.WriteLine($"Receive '{msg.CloudEvents.Count()}' messages: {cloudEvts}, BrokerName : {msg.BrokerName}, urn : {string.Join(',', msg.BridgeServers.Select(b => b.Urn))}");
    });
}
```

When both methods are added. The `RuntimeClient` instance can be configured to receive messages.

```
var runtimeClient = new RuntimeClient("localhost", 4000);
var sessionId = CreateSession(runtimeClient).Result;
var subscriptionResult = SubscribeTopic(runtimeClient, sessionId).Result;
Console.WriteLine("Please press enter to quit the application ...");
Console.ReadLine();
subscriptionResult.Stop();
runtimeClient.Disconnect("clientId", sessionId).Wait();
```