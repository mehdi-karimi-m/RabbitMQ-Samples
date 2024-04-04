// See https://aka.ms/new-console-template for more information

using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

Console.WriteLine("Consumer started.");

var factory = new ConnectionFactory
{
    HostName = "192.168.122.117",
    UserName = "test",
    Password = "Test@21"
};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
const string queueName = "Hello";
channel.QueueDeclare(queueName, false, false, false, null);

Console.WriteLine("Waiting for messages.");
var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, eventArgs) =>
{
    var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
    Console.WriteLine($"Message: '{message}' received.");
};
channel.BasicConsume(queueName, true, consumer);

Console.WriteLine("Press enter to exit.");
Console.ReadLine();
