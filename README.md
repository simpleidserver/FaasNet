# FaasNet - EventMesh implementation

[![Build status](https://ci.appveyor.com/api/projects/status/5heds9x31qc688b9?svg=true)](https://ci.appveyor.com/project/simpleidserver/faasnet)

FaasNet is an open-source implementation of EventMesh implemented in DOTNET CORE.

For project documentation, please visit [docs](https://simpleidserver.github.io/FaasNet/documentation/eventmesh/glossary.html).

## How to build a local version of EventMesh ?

```
psake publishDockerEventMeshService
docker build -t eventmesh -f EventMeshDockerFile .
docker run --name eventmesh -p 4000:4000/udp eventmesh
```