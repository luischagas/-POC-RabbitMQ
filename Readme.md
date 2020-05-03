# RabbitMQ - POC

Para execução do projeto é necessário realizar o seguinte passo:

## Instalação

Criar um container do RabbitMQ:

```bash
docker run -d -p 15672:15672 -p 5672:5672 -p 5671:5671 --hostname my-rabbitmq --name my-rabbitmq-container -e RABBITMQ_DEFAULT_USER=rabbitmq -e RABBITMQ_DEFAULT_PASS=Rabbitmq2019! rabbitmq:3-management
