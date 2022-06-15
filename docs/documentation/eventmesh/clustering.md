# Clustering

An EventMesh cluster is a logical grouping of one or several peers, each sharing Clients, Virtual Private Network, Sessions.
Any node present in the cluster must be able to process any type of request such as: add VPN client, create subscription session.
Only queues are not replicated everywhere.

Because queues are not replicated among all the nodes but only on a specific amount. The queue availability cannot be guaranteed on all the nodes.

Two types of data can easily be identified:
* Server data : data needed by a peer to process a request.
* Client data : messages that the client is interested in consuming.

For each data type, there is a different data replication strategy.

# Server data - Gossip protocol

EventMesh implements the [Gossip protocol](https://en.wikipedia.org/wiki/Gossip_protocol) to replicate the data among all the peers.

Gossip protocol is used to replicate the following data :
* Clients
* Virtual Private Network
* Sessions

# Client data - Distributed queue

To ensure fault tolerance, EventMesh replicates queues across peers using the [Raft protocol](https://en.wikipedia.org/wiki/Raft_(algorithm)).

Data is divided into partitions. Each partition has a number of replicas. Among the replica set, a leader is determined by the raft protocol which takes in requests and performs all of the processing.
All other peers are passive followers. When the leader becomes unavailable, the followers transparently select a new leader.

Eah peer in the cluster my be both leader and follower at the same time of different partitions.

In EventMesh, the partition key corresponds to the `groupId` or the `topic` name

Now, you have a better understanding of the different data replication strategy. 
We are going to explain how to form a cluster.

# Cluster Formation

An EventMesh cluster can be formed in a number of ways :
* Listing cluster nodes in configuration file (via a plugin).
* Using etcd-based discovery (via a plugin).
* Using in-memory list on nodes (enabled by default).

In order to have an up and running cluster one plugin must be enabled.
For more information please refer to the `Plugins` chapter.

# Quick start

In this tutorial, we are going to install two EventMesh servers with the plugin `DiscoveryEtcd` enabled via Docker.

TODO
