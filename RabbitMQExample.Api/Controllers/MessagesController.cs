using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQExample.Domain.Models;
using System.Threading.Tasks;
using RabbitMQExample.Domain.Enums;
using RabbitMQExample.Domain.Interfaces;

namespace RabbitMQExample.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IResponseRabbitService _responseRabbitService;
        private readonly IRabbitDatabaseProvider _rabbitDatabaseProvider;

        public MessagesController(IResponseRabbitService responseRabbitService, IRabbitDatabaseProvider rabbitDatabaseProvider)
        {
            _rabbitDatabaseProvider = rabbitDatabaseProvider;
            _responseRabbitService = responseRabbitService;
        }

        [HttpPost]
        public async Task<object> Post([FromServices] RabbitMqSettings configurations, [FromBody] Content content)
        {

            IConnection connection = _rabbitDatabaseProvider.BuildConnection(configurations);

            IModel model = connection.CreateModel();

            //Example Save Answer
            content.TypeOperation = ETypeOperation.Save;

            var sucess = await _responseRabbitService.PublishAsync(model, content);

            if (sucess)
                return new { Resultado = "Published Message." };

            return new { Resultado = "Failed To Publish Message." };
        }
    }
}