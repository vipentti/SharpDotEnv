![icon](https://raw.githubusercontent.com/vipentti/SharpDotEnv/main/icon.png)

# SharpDotEnv

SharpDotEnv is a C# library designed to support reading and loading `.env` files into the application environment.

## Features

- Load `.env` files
- Initialize environment with parsed environment variables
- Support `Microsoft.Extensions.Configuration`

## Installation

Install the NuGet package:

```sh
dotnet add package SharpDotEnv
```

To enable SharpDotEnv configuration provider install the NuGet package:

```sh
dotnet add package SharpDotEnv.Extensions.Configuration
```

## Usage

Here's a basic example of how to use SharpDotEnv:

Install the NuGet package:

```sh
dotnet add package SharpDotEnv
```

Create a new file at the root of your project called `.env`:

```dotenv
EXAMPLE_VALUE = 'this is only a test'
```

Update the `.csproj` file for your project and add the following to include the file in the build directory

```xml
  <ItemGroup>
    <None Include=".env">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
      <CopyToPublishDirectory>Never</CopyToPublishDirectory>
    </None>
  </ItemGroup>
```

```csharp
using SharpDotEnv;

// by default looks for .env file
var env = DotEnv.Config();

// Values are set in the environment by default
Console.WriteLine("value: {0}, equals: {1}",
    Environment.GetEnvironmentVariable("EXAMPLE_VALUE"),
    Environment.GetEnvironmentVariable("EXAMPLE_VALUE") == "this is only a test");

// They can also be accessed from the returned value
Console.WriteLine("value: {0}, equals: {1}",
    env["EXAMPLE_VALUE"],
    env["EXAMPLE_VALUE"] == "this is only a test");
```

See also [samples/SharpDotEnv.ConsoleSample](https://github.com/vipentti/SharpDotEnv/tree/main/samples/SharpDotEnv.ConsoleSample)

## Usage with Microsoft.Extensions.Configuration


Here's a basic example of how to use SharpDotEnv with [Microsoft.Extensions.Configuration](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration):

Install the NuGet package:

```sh
dotnet add package SharpDotEnv.Extensions.Configuration
```

Setup `.env` file following the same instructions as in [Usage](#usage).


```csharp
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddDotEnvFile(".env")
    .Build();

// Read values from the configuration
Console.WriteLine("value: {0}, equals: {1}",
    config["EXAMPLE_VALUE"],
    config["EXAMPLE_VALUE"] == "this is a nested value");
```

See also [samples/SharpDotEnv.ConfigurationSample](https://github.com/vipentti/SharpDotEnv/tree/main/samples/SharpDotEnv.ConfigurationSample)


## License

SharpDotEnv is licensed under the [MIT License](https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md)
