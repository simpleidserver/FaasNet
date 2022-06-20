# FaasNet - Serverless Functions

[![Build status](https://ci.appveyor.com/api/projects/status/5heds9x31qc688b9?svg=true)](https://ci.appveyor.com/project/simpleidserver/faasnet)

FaasNet is an open-source implementation of serverless workflow, serverless functions and EventMesh server implemented in DOTNET CORE.

For project documentation, please visit [docs](https://simpleidserver.github.io/FaasNet/documentation/bigpicture/index.html).

## Command Line

| Command                     | Description                             |
| --------------------------  | --------------------------------------- |
| psake publishWebsite 		  | Publish website                       	|
| psake publishDockerCI       | Build and publish docker images         |
| psake packTemplate          | Build template package                  |

## How to release ?

1. Build the Docker images and publish them into the Hub.

```
psake publishDockerCI
```

2. Build and publish the website.

```
psake publishWebsite
```

3. Create and publish the tag release.
4. Build the template and upload the Nuget package.

```
psake packTemplate
```

## Deploy EventMesh - Docker

psake publishDockerEventMeshService
docker build -t eventmesh -f EventMeshDockerFile .
docker run --name eventmesh -p 4000:4000/udp eventmesh