# Command-line interface

`FaasNet.EventMeshCTL.CLI.exe` is the CLI tool that ships with EventMesh. It supports a wide range of operations, mostly administrative in nature.
This includes

* Access to cluster status.
* Client and VPN management.
* Manage plugins.

## Add Virtual Private Network (VPN)

**Command**

`add_vpn`

**Description**

Add one Virtual Private Network (VPN).

**Options**

| Option    | Description    | Default value |
| --------  | -------------- | ------------- |
| -n/--name | VPN name       | default       |
| -u/--url	| EventMesh URL	 | localhost     |
| -p/--port	| EventMesh Port | 4000          |


## Get all Virtual Private Network (VPN)

**Command**

`get_all_vpn`

**Description**

Display all Message VPN.

| Option                                   | Description          | Default value |
| ---------------------------------------- | -------------------- | ------------- |
| -u/--url						           | EventMesh URL		  | localhost     |
| -p/--port								   | EventMesh Port		  | 4000          |

## Add client

**Command**

`add_client`

**Description**

Add one client.

**Options**

| Option                                   | Description          | Default value |
| ---------------------------------------- | -------------------- | ------------- |
| -v/--vpn                                 | VPN name             | default       |
| -id/--identifier                         | Client identifier    | clientId      |
| -p/--publish_enabled            		   | Can publish messages | True          |
| -s/--subscription_enabled 			   | Can subscribe        | True          |
| -u/--url						           | EventMesh URL		  | localhost     |
| -p/--port								   | EventMesh Port		  | 4000          |

## Add Message VPN bridge

**Command**

`add_vpn_bridge`

**Description**

Add bridge between two Message VPN

**Options**

| Option                                   | Description          | Default value |
| ---------------------------------------- | -------------------- | ------------- |
| -v/--vpn                                 | Source VPN           | default       |
| -tv/--tvpn                               | Target VPN           | default       |
| -tu/--turn                               | Target URN           | 		      |
| -tp/--tport                              | Target Port          | 4000	      |
| -ti/--tid				                   | Client identifier    | 		      |
| -u/--url						           | EventMesh URL		  | localhost     |
| -p/--port								   | EventMesh Port		  | 4000          |

## Display cluster status

**Command**

`cluster_status`

**Description**

Display cluster status.

**Options**

| Option                                   | Description          | Default value |
| ---------------------------------------- | -------------------- | ------------- |
| -u/--url						           | EventMesh URL		  | localhost     |
| -p/--port								   | EventMesh Port		  | 4000          |

## Get plugins

**Command**

`get_plugins`

**Description**

Display all plugins.

**Options**

| Option                                   | Description          | Default value |
| ---------------------------------------- | -------------------- | ------------- |
| -u/--url						           | EventMesh URL		  | localhost     |
| -p/--port								   | EventMesh Port		  | 4000          |

## Enable plugin

**Command**

`enable_plugin`

**Description**

Enable one plugin.

**Options**

| Option    | Description    | Default value |
| --------- | -------------- | ------------- |
| -n/--name | Plugin name 	 |               |
| -u/--url	| EventMesh URL	 | localhost     |
| -p/--port	| EventMesh Port | 4000          |

## Disable plugin

**Command**

`disable_plugin`

**Description**

Disable one plugin.

**Options**

| Option    | Description    | Default value |
| --------- | -------------- | ------------- |
| -n/--name | Plugin name    |               |
| -u/--url	| EventMesh URL	 | localhost     |
| -p/--port	| EventMesh Port | 4000          |

## Get plugin configuration

**Command**

`get_plugin_configuration`

**Description**

Get plugin configuration

**Options**

| Option    | Description    | Default value |
| --------- | -------------- | ------------- |
| -n/--name | Plugin name    |               |
| -u/--url	| EventMesh URL	 | localhost     |
| -p/--port	| EventMesh Port | 4000          |

## Update plugin configuration

**Command**

`update_plugin_configuration`

**Description**

Update plugin configuration

**Options**

| Option     | Description     | Default value |
| ---------- | --------------- | ------------- |
| -n/--name  | Plugin name     |               |
| -k/--key   | Property key    |               |
| -v/--value | Property value  |               |
| -u/--url	 | EventMesh URL   | localhost     |
| -p/--port	 | EventMesh Port  | 4000          |

## Publish one message

**Command**

`publish_message`

**Description**

Publish one message

**Options**

| Option             | Description       | Default value |
| ------------------ | ----------------- | ------------- |
| -v/--vpn	         | VPN name          | default       |
| -id/--identifier   | Client identifier | clientId      |
| -t/--topic         | Topic message     |               |
| -m/--message       | Message content   |               |
| -u/--url	         | EventMesh URL     | localhost     |
| -p/--port	         | EventMesh Port    | 4000          |

## Read one message

**Command**

`read_message`

**Description**

Read one message

**Options**

| Option             | Description       | Default value |
| ------------------ | ----------------- | ------------- |
| -v/--vpn	         | VPN name          | default       |
| -id/--identifier   | Client identifier | clientId      |
| -t/--topic         | Topic message     |               |
| -u/--url	         | EventMesh URL     | localhost     |
| -p/--port	         | EventMesh Port    | 4000          |

## Version

**Command**

`version`

**Description**

Display the CLI version.