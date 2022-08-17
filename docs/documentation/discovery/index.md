# P2P Discovery

When building a P2P Network, the biggest challenge is to choose the best method to discover Peers.

Depending on the nature of the P2P Network, the discovery method is different.

| Nature       | Discovery method                                                                                                           |
| ------------ | -------------------------------------------------------------------------------------------------------------------------- |
| Unstructured | Using storage (`configuration` or `etcd`) to store informations about the Peers OR use Broadcast protocol such as `Gossip` |
| Structured   | Discovery method is defined by the protocol. In general, Peer Identifier is used to build the Network overlay              |