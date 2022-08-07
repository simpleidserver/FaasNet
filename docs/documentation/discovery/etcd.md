# ETCD

ETCD is a distributed key-value store. It can be used by a P2P Network to store Peer Informations like Url, Port and Protocol.

## Quick start

In this tutorial we are going to explain how to have an up and running UDP Peer where Peer informations are stored in ETCD.

### Install ETCD

Ensure Docker is installed on your local machine. Run docker by executing the following command.

```
docker run -d --name Etcd-server --publish 23790:2379 --env ALLOW_NONE_AUTHENTICATION=yes bitnami/etcd:latest
```

An ETCD server instance will be launched and will listen the port `23790/TCP`.

### Create and Launch a Peer

Open a command prompt and create a console application

```
mkdir QuickStart
cd QuickStart

mkdir src
cd src

dotnet new console -n DiscoveryEtcd
```