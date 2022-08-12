# Raft consensus

Raft consensus algorithm is used to build resilient, fault-tolerant, strongly-consistent distributed systems.

Applications like Apache Kafka are using Raft Consensus algorithm for both leader election and data replication.
Instead of having all the data stored in one Peer, they are duplicated among two or more Peers.

The algorithm is widely used :
* CockroachDB uses raft in the Replication layer.
* MongoDB uses a variant of Raft in the replication set.

In the context of Raft Consensus, a Peer can have one of the following status :
* Follower : issue no requests but simply respond to request from leaders and candidates.
* Candidate : during the election process, ask to other Peers to become the leader. Candidate becomes the leader when it receives votes from majority of servers.
* Leader : handle all client requests.

![Raft consensus](images/raftconsensus.png)

## Quick start

