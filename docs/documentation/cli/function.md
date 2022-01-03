# FaasNet.CLI function

## Name

FaasNet.CLI function - Manage the lifecycle of a function : create, deploy, remove and invoke.

## Options

| Option                                                                             | Arguments                                                                                                                                                                | Description                                       |
| ---------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------- |
| `FaasNet.CLI function -df <DIR>`                                               | `DIR`: Visual studio solution directory.                                                                                                                           | Create a Docker File in the `DIR`           |
| `FaasNet.CLI function -db <DIR> <OTHER_ARGS>`                                 | `DIR`: Visual studio solution directory.<br />`OTHER_ARGS`: Other arguments passed to the command line `docker build`.                                             | Build the Docker File                             |
| `FaasNet.CLI function -dp <DIR>`                                               | `DIR`: Visual studio solution directory.                                                                                                                           | Push the docker image to the Docker hub           |
| `FaasNet.CLI function deploy -name <NAME> -image <IMAGE> -version <VERSION>`         | `NAME`: Name of the function.<br />`IMAGE`: Name of the Docker image.<br /> `VERSION`: Version of the Docker image.                                           | Deploy a function                    |
| `FaasNet.CLI function -r <ID>`                                                       | `ID`: Identifier of the function.                                                                                                                             | Remove a function                      |
| `FaasNet.CLI function invoke -id <ID> -configuration <CONFIGURATION> -input <INPT>`  | `ID`: Identifier of the function.<br />`CONFIGURATION`: Configuration of the function.<br />`INPT`: Parameters passed to the function.  | Invoke a function                      |
| `FaasNet.CLI function configuration <ID>`                                            | `ID`: Identifier of the function.                                                                                                                             | Get the configuration of the function  |