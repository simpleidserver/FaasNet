# Installing the EventMesh CLI on Windows

This guide describes how EventMesh CLI can be installed and configured manually on Windows.

The CLI is used to manage one local or remote instance of an EventMesh Peer.

## Download zip

Download the ZIP file [EventMeshCLI.zip](https://github.com/simpleidserver/FaasNet/releases/latest/download/EventMeshCLI.zip) and extract its content into a new Directory.
The windows account must have READ and WRITE access to this directory otherwise the configuration file cannot be updated or read.

## Register the service

Open the environment variable window and add the full path of this new directory into the `PATH` environment variable.

Open a new command prompt and execute `FaasNet.EventMeshCTL.CLI.exe version`. If the version is displayed then the CLI is correctly installed.

You can check if the EventMesh peer is running on your local machine by executing this command `FaasNet.EventMeshCTL.CLI.exe cluster_status`.
You should see at least one instance listening on the port 4000.

```
Current nodes
Url=localhost, Port=4000
```