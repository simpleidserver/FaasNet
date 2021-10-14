# Create a function

Install FaasNet template. This utility can be used to generate Function project in C#.

```
dotnet new --install FaasNet.Templates
```

Create a Function project

```
mkdir QuickStart
cd QuickStart

mkdir src
cd src

dotnet new faasnetfn -n Function
```

The following files will be created :

* *Startup.cs* and *Program.cs*: the application entry point.
* *HelloWorldConfiguration.cs*: configuration properties of the function.
* *FunctionHandler.cs*: contains the business logic. This class has one function which accepts one parameter and returns a JSON result. The input parameter has two distinct properties :

  * Configuration: its value is coming from the gateway, it will be used to configure the behavior of the function for example : `ConnectionString` and `SQL Statement`.
  * Input: value passed by caller.

In case the Visual Studio Support is needed, a solution can be created :

```
cd ..
dotnet new sln -n QuickStart
```

Add the Function project into the solution.

```
dotnet sln add ./src/Function/Function.csproj
```

# Deploy a function

> [!WARNING]
> Before you start, Make sure your working environment is properly configured.

When the Function project is ready, it can be deployed to the Gateway API.

First of all, open a command prompt and execute the following command line to create a Docker file. The `DIRECTORY` variable must be replaced by the directory of the Function.csproj project.

```
FaasNet.CLI function -df <DIRECTORY>
```

Execute the following instruction to locally build the Docker image. 
The `DIRECTORY` variable must be replaced by the directory of the Function.csproj project, and the `IMAGENAME` variable must be replaced by the name of the Docker image for example : localhost:5000/function.

```
FaasNet.CLI function -db <DIRECTORY> -t <IMAGENAME>
```

Execute the following command line to push the local Docker image into a registry. The `IMAGENAME` variable must be replaced by the name of the Docker Image.

```
FaasNet.CLI function -dp <IMAGENAME>
```

Finally, execute the latest command line to deploy the function into the Gateway API. Replace the `FUNCTIONNAME` variable by the name of your function and replace the `IMAGENAME` variable by the name of your Docker image.

```
FaasNet.CLI function deploy -name <FUNCTIONAME> -image <IMAGENAME>
```

# Execute a function

> [!WARNING]
> Before you start, Make sure you have a function deployed in the Gateway API.

Execute the following command to invoke a function. Replace the `FUNCTIONNAME` variable by the name of your function.

```
FaasNet.CLI function invoke -name <FUNCTIONNAME> -input {} -configuration {'firstName':'coucou'}
```

The following message is displayed

```
{
  "content": {
    "message": "Hello 'coucou'"
  }
}
```