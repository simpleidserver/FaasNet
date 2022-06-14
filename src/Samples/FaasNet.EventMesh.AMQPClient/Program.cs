// See https://aka.ms/new-console-template for more information
using Amqp;
const int port = 5672;
ReceiveMessage();
SendMessage();
Console.WriteLine("Press enter to quit the application");
Console.ReadLine();

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