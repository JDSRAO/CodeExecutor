# Dynamic Code Executor

[![NuGet version (CodeExecutor)](https://img.shields.io/nuget/v/CodeExecutor.svg?style=flat-square)](https://www.nuget.org/packages/CodeExecutor/)

This repository contains helper dotnet classess for dynamic code compilation and execution.

Supported languages 
- C#
- JavaScript

## Help us improve
Help us improve by sending us pull-requests or opening a [GitHub Issue](https://github.com/JDSRAO/CodeExecutor/issues)

## Table of Contents  
1. [Development](#development)
2. [Usage](#usage)
3. [License](#license)

## Development
To use the samples with Git, clone the project repository with `git clone https://github.com/JDSRAO/CodeExecutor`

After cloning the respository:
* To build the samples, open `CodeExecutor.sln` solution file in Visual Studio 2017 and build the solution.
* Alternatively, open the project directory in command prompt and type ``` cd CodeExecutor ``` and build with 'dotnet build' or 'msbuild' specifying the target project file.

The easiest way to use these samples without using Git is to download the zip file containing the current version. You can then unzip the entire archive and use the solution in Visual Studio 2017.

## Usage
- Download the latest release from [here](https://github.com/JDSRAO/CodeExecutor/releases).
- Alternatively download the latest [nuget package](https://www.nuget.org/packages/CodeExecutor/), install and start development.
- The API's are tailor made to meet all the common requirements.

### JavaScript Interpreter 
- The API uses [Jnit](https://github.com/sebastienros/jint) to execute JavaScript.
- The API takes the name of the function to execute, code to execute and input arguments (if any).


### C# Executor
- The API uses [Roslyn](https://github.com/dotnet/roslyn) to compile and execute c# code.
- The API takes class name with namespace, the name of the function to execute, code to execute and input arguments (if any).

## License
Please refer [here](LICENSE) for license information
