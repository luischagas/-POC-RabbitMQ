using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using RabbitMQExample.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using RabbitMQExample.Domain.Interfaces;

namespace RabbitMQExample.Infrastructure.Rabbit.Providers
{
    public class RabbitDatabaseProvider : IRabbitDatabaseProvider
    {
        #region Constructors

        public RabbitDatabaseProvider(RabbitMqSettings configurations)
        {
            IConnection connection = BuildConnection(configurations);

            IModel model = connection.CreateModel();

            model.QueueDeclare(configurations.DeadLetterQueueName, true, false, false, null);
            model.ExchangeDeclare(configurations.DeadLetterExchangeName, "direct", true);
            model.QueueBind(configurations.DeadLetterQueueName, configurations.DeadLetterExchangeName, configurations.DeadLetterRoutingKeyValue, null);

            model.ExchangeDeclare(configurations.DefaultExchangeName, "direct", true);
            model.QueueDeclare
            (
                configurations.DefaultQueueName, true, false, false,
                new Dictionary<string, object>
                {
                    {"x-dead-letter-exchange", configurations.DeadLetterExchangeName},
                    {"x-dead-letter-routing-key", configurations.DeadLetterRoutingKeyValue}
                }
            );
            model.QueueBind(configurations.DefaultQueueName, configurations.DefaultExchangeName, configurations.DefaultQueueName, null);
        }

        #endregion Constructors

        #region Methods

        public IConnection BuildConnection(RabbitMqSettings configurations)
        {
            ConnectionFactory factory = new ConnectionFactory
            {
                HostName = configurations.HostName,
                UserName = configurations.UserName,
                Password = configurations.Password,
                Port = configurations.Port,
                DispatchConsumersAsync = true
            };

            IConnection conn;
            try
            {
                conn = factory.CreateConnection();
            }
            catch (BrokerUnreachableException rootException) when (DecomposeExceptionTree(rootException).Any(it => it is ConnectFailureException && (it.InnerException?.Message?.Contains("Connection refused") ?? false)))
            {
                throw new Exception();
            }

            return conn;
        }

        private static IEnumerable<Exception> DecomposeExceptionTree(Exception currentException)
        {
            while (currentException != null)
            {
                yield return currentException;
                currentException = currentException.InnerException;
            }
        }

        #endregion Methods
    }
}