using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQExample.Domain.Interfaces;
using RabbitMQExample.Domain.Models;

namespace RabbitMQExample.Infrastructure.Rabbit
{
    public class ResponseRabbitService : IResponseRabbitService
    {
        #region Methods

        public async Task<bool> PublishAsync(IModel model, object content)
        {
            model.ConfirmSelect();

            var body = JsonConvert.SerializeObject(content);

            IBasicProperties props = model.CreateBasicProperties();
            props.Persistent = true;

            var testMessage = JsonConvert.DeserializeObject<Content>(body);

            var cacheKey = GenerateCacheKey(testMessage);

            try
            {
                model.BasicPublish("MessagesExchange", "MessageQueue", props, Encoding.UTF8.GetBytes(body));
                model.WaitForConfirmsOrDie();
            }
            catch (Exception ex)
            {
                return false;
            }

            await Task.Yield();

            return true;
        }

        public async Task ConsumerAsync(IModel model)
        {
            var consumer = new AsyncEventingBasicConsumer(model)
            {
                ConsumerTag = Guid.NewGuid().ToString()
            };

            consumer.Received += (o, args) => HandleMessageAsync(args, model);

            //Register consumer in queue
            model.BasicConsume("MessageQueue", false, consumer);

            await Task.Yield();
        }

        private async Task HandleMessageAsync(BasicDeliverEventArgs ea, IModel model)
        {
            var memoryDictionary = new Dictionary<Guid, object>();

            try
            {
                var brokerMessage = Encoding.Default.GetString(ea.Body);

                Console.WriteLine($"New message received: {brokerMessage}, by the consumer: {ea.ConsumerTag}");

                var testMessage = JsonConvert.DeserializeObject<Content>(brokerMessage);

                if (testMessage != null)
                {
                    var cacheMemory = Guid.NewGuid();
                    memoryDictionary.Add(cacheMemory, testMessage);

                    var success = memoryDictionary.TryGetValue(cacheMemory, out _);

                    if (testMessage.Name == "Luis")
                        success = false;

                    if (!success)
                    {
                        //log
                    }

                    //Tell RabbitMQ that the message was successfully sent by the consumer
                    if (success)
                        model.BasicAck(ea.DeliveryTag, false);
                    else
                    {
                        if (ea.Redelivered is false)
                            model.BasicNack(ea.DeliveryTag, false, true);
                        else
                            model.BasicNack(ea.DeliveryTag, false, false);
                    }
                }

                await Task.Yield();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static string GenerateCacheKey(Content content)
        {
            var keyBuilder = new StringBuilder();

            keyBuilder.Append($"{content.InterviewId}_{content.AnswerId}_{content.TypeOperation}");

            return keyBuilder.ToString();
        }

        #endregion Methods
    }
}