# Installation

> [!NOTE]
> The source code of this project can be found [here](https://github.com/simpleidserver/FaasNet/tree/master/samples/EventMeshServerInMemory).

A standalone EventMesh Server can be hosted in a console application.

```
mkdir QuickStart
cd QuickStart

mkdir src
cd src

dotnet new evtmeshinmem -n EventMeshServer
```

A `Program.cs` file will be created. It is the application entry point. It contains the configuration of the EventMesh server like : `urn` and `port`.

In case the Visual Studio Support is needed, a solution can be created :

```
cd ..
dotnet new sln -n QuickStart
```

Add the EventMesh server into the solution :

```
dotnet sln add ./src/EventMeshServer/EventMeshServer.csproj
```

Run the server. By default the server is listening on `localhost:4000`.

```
cd src/EventMeshServer
dotnet run
```

![Architecture](images/installation-1.png)