using System.Text;
using Common.Config;
using RabbitMQ.Client;

Console.WriteLine("Topic log producer started!");
Console.Title = "Topic log producer";

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

Console.WriteLine("Enter log message:");
var logMessage = Console.ReadLine();
Console.WriteLine("Enter routing key:");
var routingKey = Console.ReadLine();
routingKey = !string.IsNullOrWhiteSpace(routingKey) ? routingKey : "anonymous.info";

while (!string.IsNullOrWhiteSpace(logMessage))
{
    var body = Encoding.UTF8.GetBytes(logMessage);

    channel.BasicPublish(exchangeName, routingKey, basicProperties: null, body);
    
    Console.WriteLine("Enter log message:");
    logMessage = Console.ReadLine();
    Console.WriteLine("Enter routing key:");
    routingKey = Console.ReadLine();
    routingKey = !string.IsNullOrWhiteSpace(routingKey) ? routingKey : "anonymous.info";
}

Console.WriteLine("Press enter to exit.");
Console.ReadLine();