using System.Text;
using Common.Config;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

Console.WriteLine("Task consumer started!");

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

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, eventArgs) =>
{
    var task = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
    Console.WriteLine($"Task {task} received.");
    var taskArray = task.Split('.');
    Thread.Sleep(taskArray.Length * 1000);
    Console.WriteLine($"{task} processed.");
    Console.WriteLine("---------------------");
    if (sender is EventingBasicConsumer eventingBasicConsumer)
    {
        eventingBasicConsumer.Model.BasicAck(eventArgs.DeliveryTag, false);
    }
};
channel.BasicConsume(queueName, false, consumer);

Console.WriteLine("Press enter to exit.");
Console.ReadLine();