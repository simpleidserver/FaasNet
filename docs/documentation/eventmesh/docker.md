# Docker

An EventMesh server can easily be installed with Kubernetes. There is one deployment file per type of message broker. Following chapters list the different possibilites.

Whatever the deployment file chosen. By default, the EventMesh server is listening on TCP Port `30005` and a UI portal is available [here](http://localhost:30004).

## InMemory

An EventMesh server plugged with an InMemory message broker can be deployed like this :

```
kubectl apply -f https://raw.githubusercontent.com/simpleidserver/FaasNet/master/kubernetes/release/eventmeshserver.yml --namespace=faas
```

## RabbitMQ

An EventMesh server plugged with RabbitMQ can be deployed like this :

```
kubectl apply -f https://raw.githubusercontent.com/simpleidserver/FaasNet/master/kubernetes/release/eventmeshserver.rabbitmq.yml --namespace=faas
```

## Kafka

An EventMesh server plugged with Apache Kafka can be deployed like this :

```
kubectl apply -f https://raw.githubusercontent.com/simpleidserver/FaasNet/master/kubernetes/release/eventmeshserver.kafka.yml --namespace=faas
```