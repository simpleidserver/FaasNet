# Installing on Windows Manually

This guide describes how EventMesh server can be installed and configured manually on Windows.

## Download zip

Download the zip file [EventMeshServer.zip](TODO) and extract its content into a new directory.

## Deploy the windows service

As an administrator, open a command prompt and execute the following command. The parameter `$result_dir` must be replaced by the full path of the new directory.
The windows account used to run the windows service must have READ and WRITE access to this new directory otherwise the key-value storage cannot be created.

```
sc.exe create "EventMesh Service" binpath="$result_dir\FaasNet.EventMesh.Service.exe --contentRoot $result_dir"
```

In the same command prompt. Execute the command below to start the windows service.

```
sc.exe start "EventMesh Service"
```

The windows service can be stopped like this :

```
sc.exe stop "EventMesh Service"
```

After some minutes, the EventMesh server is launched and is listening on the port 4000.
Now, the CLI can be downloaded to manage the local EventMesh server.