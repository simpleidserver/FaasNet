# Create a function

Install FaasNet template

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
* *HelloWorldConfiguration.cs*: configuration of the function.
* *FunctionHandler.cs*: logic of the function.

In case the Visual Studio Support is needed, a solution can be created :

```
cd ..
dotnet new sln -n QuickStart
```

Add the Function project into the solution :

```
dotnet sln add ./src/Function/Function.csproj
```

# Deploy a function

Create the docker file. Replace the `DIRECTORY` variable by the project directory.

```
FaasNet.CLI function -df <DIRECTORY>
```

Build the docker image. Replace the `DIRECTORY` variable by the project directory, replace the `NAME` variable by the name of your image. 

```
FaasNet.CLI function -db <DIRECTORY> -t <NAME>
```

Push the docker image into the Hub. Replace the `NAME` variable by the name of your image.

```
FaasNet.CLI function -dp <NAME>
```