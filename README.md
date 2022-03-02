# FaasNet - Serverless Functions

[![Build status](https://ci.appveyor.com/api/projects/status/5heds9x31qc688b9?svg=true)](https://ci.appveyor.com/project/simpleidserver/faasnet)

FaasNet makes it easy to deploy functions and API to Kubernetes without repetitive, boiler-plate coding.

For project documentation, please visit [docs](https://simpleidserver.github.io/FaasNet/).

## Command Line

| Command                     | Description                             |
| --------------------------  | --------------------------------------- |
| psake publishWebsite 		  | Publish website                         |
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