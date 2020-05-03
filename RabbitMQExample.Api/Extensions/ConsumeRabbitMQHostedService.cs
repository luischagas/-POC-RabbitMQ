using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQExample.Domain.Interfaces;
using RabbitMQExample.Domain.Models;

namespace RabbitMQExample.Api.Extensions
{
    public class ConsumeRabbitMqHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly RabbitMqSettings _configurations;
        private IConnection _connection;
        private IModel _model;
        private IResponseRabbitService _responseRabbitService;
        private IRabbitDatabaseProvider _rabbitDatabaseProvider;

        public ConsumeRabbitMqHostedService(IServiceScopeFactory serviceScopeFactory, RabbitMqSettings configurations)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _configurations = configurations;
            InitRabbitMq();
        }

        private void InitRabbitMq()
        {
            var dbContext = _serviceScopeFactory.CreateScope();

            _rabbitDatabaseProvider = dbContext.ServiceProvider.GetRequiredService<IRabbitDatabaseProvider>();

            _responseRabbitService = dbContext.ServiceProvider.GetRequiredService<IResponseRabbitService>();

            _connection = _rabbitDatabaseProvider.BuildConnection(_configurations);

            _model = _connection.CreateModel();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            _responseRabbitService.ConsumerAsync(_model);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _model.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}