# Command-line interface

`FaasNet.EventMeshCTL.CLI.exe` is the CLI tool that ships with EventMeshServer. It supports a wide range of operations, mostly administrative in nature.
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

| Option    | Description  | Default value |
| --------  | ------------ | ------------- |
| -n/--name | VPN name     | default       |


## Get all Virtual Private Network (VPN)

**Command**

`get_all_vpn`

**Description**

Display all the VPN.

## Add client

**Command**

`add_client`

**Description**

Add one client.

**Options**

| Option                                   | Description          | Default value |
| ---------------------------------------- | -------------------- | ------------- |
| -v/--vpn                                 | VPN name             | default       |
| -id/--identifier <LOGIN>                 | Client identifier    | clientId      |
| -p/--publish_enabled <PUBLISH>           | Can publish messages | True          |
| -s/--subscription_enabled <SUBSCRIPTION> | Can subscribe        | True          |

## Display cluster status

**Command**

`cluster_status`

**Description**

Display cluster status.

## Get plugins

**Command**

`get_plugins`

**Description**

Display all plugins.

## Enable plugin

**Command**

`enable_plugin`

**Description**

Enable one plugin.

**Options**

| Option    | Description | Default value |
| --------- | ----------- | ------------- |
| -n/--name | Plugin name |               |

## Disable plugin

**Command**

`disable_plugin`

**Description**

Disable one plugin.

**Options**

| Option    | Description | Default value |
| --------- | ----------- | ------------- |
| -n/--name | Plugin name |               |

## Version

**Command**

`version`

**Description**

Display the CLI version.