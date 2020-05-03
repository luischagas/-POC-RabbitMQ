using RabbitMQ.Client;
using RabbitMQExample.Domain.Models;

namespace RabbitMQExample.Domain.Interfaces
{
    public interface IRabbitDatabaseProvider
    {
        IConnection BuildConnection(RabbitMqSettings configurations);
    }
}