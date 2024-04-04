using System.Text;
using Common.Config;
using RabbitMQ.Client;

Console.WriteLine("Routing log producer started!");

var factory = new ConnectionFactory
{
    HostName = RabbitMqConfig.HostName,
    UserName = RabbitMqConfig.UserName,
    Password = RabbitMqConfig.Password
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

const string exchangeName = "direct_logs";
channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);

Console.WriteLine("Enter log message:");
var logMessage = Console.ReadLine();
var randomLogType = (LogType)new Random(Guid.NewGuid().GetHashCode()).Next(1, 6);
while (!string.IsNullOrWhiteSpace(logMessage))
{
    var body = Encoding.UTF8.GetBytes(logMessage);
    var routingKey = randomLogType.ToString();
    channel.BasicPublish(exchangeName, routingKey, basicProperties: null, body);
    Console.WriteLine($"Log message: {logMessage} with severity: {routingKey} published.");
    Console.WriteLine("Enter log message:");
    logMessage = Console.ReadLine();
    randomLogType = (LogType)new Random(Guid.NewGuid().GetHashCode()).Next(1, 6);
}