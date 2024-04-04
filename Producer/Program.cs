// See https://aka.ms/new-console-template for more information

using System.Net.Mime;
using System.Text;
using Common.Config;
using RabbitMQ.Client;

Console.WriteLine("Producer started!");

var factory = new ConnectionFactory
{
    HostName = RabbitMqConfig.HostName,
    UserName = RabbitMqConfig.UserName,
    Password = RabbitMqConfig.Password
};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
const string queueName = "Hello";
channel.QueueDeclare(queueName, false, false, false, null);

var message = "Hello world";
do
{
    var body = Encoding.UTF8.GetBytes(message);
    channel.BasicPublish(string.Empty, queueName, basicProperties: null, body: body);
    Console.WriteLine($"message: '{message}' sent.");

    Console.WriteLine("Enter new message:");
    message = Console.ReadLine();
} while (!string.IsNullOrWhiteSpace(message));