using System.Text;
using Common.Config;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

Console.WriteLine("Topic log consumer started!");
Console.Title = "Topic log consumer";

var factory = new ConnectionFactory()
{
    HostName = RabbitMqConfig.HostName,
    UserName = RabbitMqConfig.UserName,
    Password = RabbitMqConfig.Password
};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

const string exchangeName = "topic_logs";
channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);

if (args.Length <= 0)
{
    Console.Error.WriteLine("Routing key wasn't provide.");
    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
    Environment.ExitCode = 1;
    return;
}
var queueName = channel.QueueDeclare().QueueName;
foreach (var routingKey in args)
{
    channel.QueueBind(queueName, exchangeName, routingKey: routingKey);    
}

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, eventArgs) =>
{
    var logMessage = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
    Console.WriteLine($"{eventArgs.RoutingKey} - message: {logMessage}");
    Console.WriteLine("-----------------");
    if(sender is EventingBasicConsumer eventingBasicConsumer)
    {
        eventingBasicConsumer.Model.BasicAck(eventArgs.DeliveryTag, false);
    }
};
channel.BasicConsume(queueName, false, consumer);

Console.WriteLine("Press enter to exit.");
Console.ReadLine();