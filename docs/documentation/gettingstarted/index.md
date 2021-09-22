# Before you start

Make sure you have [Kubernetes](https://kubernetes.io/docs/tasks/tools/) and [Helm](https://helm.sh/docs/intro/install/) installed.

Download and install the FaasNet CLI tool :

1. Download your [desired version](https://github.com/simpleidserver/FaasNet/releases).
2. Unpack it.
3. Add the Directory path into the PATH environment variable.

# Start FaasNet

Open a command prompt and create a `faas` namespace in kubernetes. 

```
kubectl create namespace faas
```

Install and launch the `faasnet` project.

```
helm repo add faasnet https://simpleidserver.github.io/FaasNet/charts/
helm install my-faasnet faasnet/faasnet --version 0.0.1 --namespace=faas
```

# Deploy and execute your first API operation

Before starting, an SQLServer database should be deployed in kubernetes.

```
kubectl apply -f https://raw.githubusercontent.com/simpleidserver/FaasNet/master/kubernetes/run-mssql.yml --namespace=faas
kubectl apply -f https://raw.githubusercontent.com/simpleidserver/FaasNet/master/kubernetes/mssql-external-svc.yml --namespace=faas
kubectl apply -f https://raw.githubusercontent.com/simpleidserver/FaasNet/master/kubernetes/mssql-internal-svc.yml --namespace=faas
```

Authenticate to the SQLServer database `127.0.0.1, 30002` with the following credentials :

| Parameter | Value        |
| --------- | ------------ |
| Login     | sa           |
| Password  | D54DE7hHpkG9 |

Add a new `OpenID` database schema, create the table `[dbo].[Acrs]` and insert some data.

```
CREATE DATABASE OpenID;
CREATE TABLE [OpenID].[dbo].[Acrs](
	[Id] [uniqueidentifier] NULL,
	[Name] [nvarchar](255) NULL
) ON [PRIMARY]
GO
INSERT INTO [OpenID].[dbo].[Acrs] VALUES (NEWID(), 'acr1')
INSERT INTO [OpenID].[dbo].[Acrs] VALUES (NEWID(), 'acr2')
INSERT INTO [OpenID].[dbo].[Acrs] VALUES (NEWID(), 'acr3')
```

Deploy your first API operation. If there is no error during the deployment then the message `Configuration is applied` is displayed.

```
FaasNet.CLI.exe apply -u https://raw.githubusercontent.com/simpleidserver/FaasNet/master/faasnet.yml
```

Open Postman or an another tool, execute the HTTP POST request against `http://localhost:30001/clients`.
