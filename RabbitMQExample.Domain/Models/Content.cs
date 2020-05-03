using System;
using RabbitMQExample.Domain.Enums;

namespace RabbitMQExample.Domain.Models
{
    public class Content
    {
        public Guid InterviewId { get; set; }

        public Guid AnswerId { get; set; }

        public ETypeOperation TypeOperation { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }
    }
}