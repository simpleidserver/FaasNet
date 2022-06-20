# Installing on Docker

This guide describes how EventMesh peer node can be installed on Docker.

## Deploy a windows service

> [!IMPORTANT]
> Docker must be installed on your machine.

Open a command prompt and execute the following command line. An EventMesh peer node will be deployed and listening the port 4000.

```
docker run --name eventmesh -p 4000:4000/udp eventmesh
```