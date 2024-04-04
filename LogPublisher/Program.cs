using System.Text;
using Common.Config;
using RabbitMQ.Client;

Console.WriteLine("Log publisher started!");

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

Console.WriteLine("Enter log message:");
var logMessage = Console.ReadLine();
while (!string.IsNullOrWhiteSpace(logMessage))
{
    var data = Encoding.UTF8.GetBytes(logMessage);
    channel.BasicPublish(exchangeName, routingKey: string.Empty, body: data);
    
    Console.WriteLine("Press enter to exit or enter log message to publish:");
    logMessage = Console.ReadLine();
}