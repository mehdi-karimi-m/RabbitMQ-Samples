using System.Text;
using Common.Config;
using RabbitMQ.Client;

Console.WriteLine("Task producer started!");

var factory = new ConnectionFactory()
{
    HostName = RabbitMqConfig.HostName,
    UserName = RabbitMqConfig.UserName,
    Password = RabbitMqConfig.Password
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
const string queueName = "Tasks_Queue";
channel.QueueDeclare(queueName, true, false, false, null);
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, false);

var task = "Task";
var counter = 1;
do
{
    task = task.PadRight(counter, '.');
    var body = Encoding.UTF8.GetBytes(task);
    var properties = channel.CreateBasicProperties();
    properties.Persistent = true;
    channel.BasicPublish(string.Empty, queueName, basicProperties: properties, body);
    Console.WriteLine($"Task {counter}: '{task}' sent.");

    Console.WriteLine("Enter new task:");
    task = Console.ReadLine();
    counter++;
} while (!string.IsNullOrWhiteSpace(task));

