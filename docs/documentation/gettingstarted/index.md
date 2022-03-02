# Before you start

Make sure you have [Kubernetes](https://kubernetes.io/docs/tasks/tools/) installed.

Download and install the FaasNet CLI tool :

1. Download your [desired version](https://github.com/simpleidserver/FaasNet/releases).
2. Unpack it.
3. Add the Directory path into the PATH environment variable.

This utility can be used to create and deploy serverless functions.

# Start FaasNet

Open a command prompt and execute the following command line.

```
kubectl apply -f https://raw.githubusercontent.com/simpleidserver/FaasNet/master/kubernetes/release/serverlessworkflow.yml --namespace=faas
```

The project will be deployed into Kubernetes. 

A portal is available [here](http://localhost:30003), it can be used to perform some administrative tasks like : managing `functions` and `serverless workflows`.


# Template

A DOTNET project template can be installed. 

It contains a list of projects which can be used to create EventMesh servers and serverless functions.


```
dotnet new --install FaasNet.Templates
```

| Command line               | Description                                                         |
|  ------------------------- | ------------------------------------------------------------------- |
| dotnet new evtmeshinmem    | Standalone EventMesh server plugged with an InMemory broker         |
| dotnet new evtmeshkafka    | Standalone EventMesh server plugged with an Apache Kafka broker     |
| dotnet new evtmeshrabbitmq | Standalone EventMesh server plugged with a RabbitMQ broker          |
| dotnet new evtmeshinmemui  | EventMesh server with a UI plugged with an InMemory broker          |
| dotnet new faasnetfn       | Empty function project                                              |