namespace RabbitMQExample.Domain.Models
{
    public class RabbitMqSettings
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DefaultExchangeName { get; set; }
        public string DefaultQueueName { get; set; }
        public string DeadLetterExchangeName { get; set; }
        public string DeadLetterQueueName { get; set; }
        public string DeadLetterRoutingKeyValue { get; set; }
    }
}