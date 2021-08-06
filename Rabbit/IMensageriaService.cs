using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rabbit
{
    public interface IMensageriaService
    {
        bool isConnected();
        void openConnection(Action<bool> onExecute);
        void closeConnection();
        void createQueue(string queueName);
        void deliverMessage(string message, string exchange, string routingKey);
        void receiveMessage(string queueName, EventHandler<BasicDeliverEventArgs> onReceived);

    }
}
