Deploy one RabbitMQ server with AMQP1.0 support

```
docker build -t amqpserver -f AmqpServerDockerFile .
docker run --name rabbitmq -p 5672:5672 -p 15672:15672 amqpserver
``` 