using RabbitMQ.Client;
using System.Threading.Tasks;

namespace RabbitMQExample.Domain.Interfaces
{
    public interface IResponseRabbitService
    {
        Task<bool> PublishAsync(IModel model, object content);

        Task ConsumerAsync(IModel model);
    }
}