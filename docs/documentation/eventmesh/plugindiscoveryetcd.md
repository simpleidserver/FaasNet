# Etcd Peer Discovery

**Name**

DiscoveryEtcd

**Description**

Use Etcd to discover peers.

**Link**

The ZIP file can be downloaded [here]().

**Options**

| Name                  | Description                  	| Default value 			 |
| --------------------- | ----------------------------- | -------------------------- |
| etcdConnectionString 	| Etcd connection string        | http://localhost:23790     |
| etcdUsername 			| Etcd Username					| 							 |
| etcdPassword 			| Etcd Password					| 							 |
| etcdPrefix 			| URL prefix					| eventmesh					 |

## Quick start

In this tutorial, we are going to build an EventMesh cluster with two Peers by using Docker. 

The plugin `DiscoveryEtcd` is used as a discovery method.

Download the docker compose file 

```
https://raw.githubusercontent.com/simpleidserver/FaasNet/master/src/Samples/FaasNet.EventMesh.EtcdCluster/docker-compose.yml
```

Open a command prompt and execute the following command 

```
docker-compose up
```

Two peers will be deployed and will listen the ports `4000` and `4001`.

Always in a command prompt, add a new Message VPN 

```
FaasNet.EventMeshCTL.CLI.exe add_vpn --name=vpn --port=4001
```

Check if the Message VPN is correctly replicated in the second node by executing the following command 

```
FaasNet.EventMeshCTL.CLI.exe get_all_vpn --port=4002
```