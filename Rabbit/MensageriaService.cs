using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Rabbit
{
    public class MensageriaService : IMensageriaService
    {
        private ConnectionFactory Factory;
        private IConnection Connection;
        private IModel Channel;

        public MensageriaService(string hostName)
        {
            Factory = new ConnectionFactory() { HostName = hostName };
            Factory.AutomaticRecoveryEnabled = true;
        }

        public void createQueue(string queueName)
        {
            Channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        public void deliverMessage(string message, string exchange, string routingKey)
        {
            var body = MessageToByte(message);
            Channel.BasicPublish(exchange: exchange,
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);

        }


        public void receiveMessage(string queueName, EventHandler<BasicDeliverEventArgs> onReceived)
        {
            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += onReceived;
            Channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);
        }

        public void openConnection(Action<bool> onExecute)
        {
            while (Connection == null)
            {
                try
                {
                    Connection = Factory.CreateConnection();
                    Channel = Connection.CreateModel();
                    onExecute(true);
                }
                catch (Exception e)
                {
                    onExecute(false);
                    Thread.Sleep(5000);
                }
            }
        }

        public void closeConnection()
        {
            if (Channel.IsOpen)
            {
                Channel.Close();
                Connection.Close();
            }
        }

        public bool isConnected()
        {
            return Connection.IsOpen;
        }
        private byte[] MessageToByte(string message)
        {
            return Encoding.UTF8.GetBytes(message);
        }
    }
}
