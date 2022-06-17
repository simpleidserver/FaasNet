# EventMesh glossary and concepts

## Peer node

A Peer Node is a communication endpoint capable of treating EventMesh, Raft and Gossip request.

Two or more Peers can form a cluster, they all share the same functionality.

## Cluster

A cluster network is two or more services working together for a common purpose.

These networks take advantage of the parallel processing power. In addition to the increased processing power, can also provide scalability, high availability and failover capabilities.

## Client

Any types of application that implement the EventMesh protocol.

A client is capable of sending and/or receiving messages.

## Session

A session is a communication channel established between one Peer Node and a client.

It is used to send and receive messages.

There are two kinds of session :
* **Subscription** : Subscribe to one or more topics.
* **Publish** : Publish one or more messages.

## Topic

Topic is a mean of classifying information. 

## Message Virtual Private Network (VPN)

Allows for the segregation of topic space and clients. Message VPNs also group clients connecting to a network of event broker, such that messages published within a particular group are only visible to that group's clients.

## Message VPN bridge

Link two Message VPNs so that messages published to one message VPN that match the topic subscriptions set for the bridge are also delivered to the linked message VPN.

## CloudEvent

Specification for describing event data in a common way.