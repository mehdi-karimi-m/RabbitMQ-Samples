using System.Text;
using Common.Config;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

Console.WriteLine("Routing log consumer started!");

var factory = new ConnectionFactory()
{
    HostName = RabbitMqConfig.HostName,
    UserName = RabbitMqConfig.UserName,
    Password = RabbitMqConfig.Password
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

const string exchangeName = "direct_logs";
channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);

Console.WriteLine("Enter log types that you want to get (Verbose,Debug,Information,Warning,Error,Fatal):");
var logTypesText = Console.ReadLine();
if (string.IsNullOrWhiteSpace(logTypesText)) return;

var arrayOfLogTypeText = logTypesText.Split(',');
if (arrayOfLogTypeText.Length <= 0) return;

var logTypes = new List<LogType>();
foreach (var logTypeText in arrayOfLogTypeText)
{
    if (Enum.TryParse(logTypeText, out LogType logType))
        logTypes.Add(logType);
}

var queueName = channel.QueueDeclare().QueueName;
foreach (var logType in logTypes)
{
    channel.QueueBind(queueName, exchangeName, logType.ToString(), null);
}

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, eventArgs) =>
{
    var logMessage = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
    if (Enum.TryParse(eventArgs.RoutingKey, out LogType logType))
    {
        Logger.Log(logType, logMessage);
    }

    if (sender is EventingBasicConsumer eventingBasicConsumer)
    {
        eventingBasicConsumer.Model.BasicAck(eventArgs.DeliveryTag, false);
    }
};
channel.BasicConsume(queueName, false, consumer);

Console.WriteLine("Press enter to exit.");
Console.ReadLine();