using System.Text;
using Common.Config;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

Console.WriteLine("Log subscriber started!");

Console.WriteLine("enter 'f' to write to file or enter 'p' to print the log messages:");
var actionType = Console.ReadLine();

var factory = new ConnectionFactory()
{
    HostName = RabbitMqConfig.HostName,
    UserName = RabbitMqConfig.UserName,
    Password = RabbitMqConfig.Password
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

const string exchangeName = "logs";
channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout);

var queueName = channel.QueueDeclare().QueueName;
channel.QueueBind(queueName, exchangeName, routingKey: string.Empty);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, eventArgs) =>
{
    var logMessage = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

    Console.WriteLine(actionType == "f" ? $"'{logMessage}' wrote to file." : logMessage);

    if (sender is not EventingBasicConsumer eventingBasicConsumer) return;
    
    var model = eventingBasicConsumer.Model;
    model.BasicAck(eventArgs.DeliveryTag, false);
};
channel.BasicConsume(queueName, false, consumer);

Console.WriteLine("Press enter to exit.");
Console.ReadLine();