Open a command prompt and install the RabbitMQ Cluster Kubernetes Operator

```
kubectl apply -f "https://github.com/rabbitmq/cluster-operator/releases/latest/download/cluster-operator.yml"
```

Deploy RabbitMQ

```
kubectl apply -f "https://raw.githubusercontent.com/simpleidserver/FaasNet/master/src/Samples/FaasNet.EventMesh.AmqpSink/rabbitmq.yml"
```