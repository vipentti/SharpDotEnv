// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using SharpDotEnv;

var env = DotEnv.Config(new DotEnvConfigOptions().WithPath(".env-sample").WithDebug(true));

// Values are set in the environment by default
Console.WriteLine(
    "value: {0}, equals: {1}",
    Environment.GetEnvironmentVariable("EXAMPLE_VALUE"),
    Environment.GetEnvironmentVariable("EXAMPLE_VALUE") == "this is only a test"
);

// They can also be accessed from the returned value
Console.WriteLine(
    "value: {0}, equals: {1}",
    env["EXAMPLE_VALUE"],
    env["EXAMPLE_VALUE"] == "this is only a test"
);

Console.WriteLine(
    "value: {0}, equals: {1}",
    Environment.GetEnvironmentVariable("ExampleSettings__Nested__Value"),
    Environment.GetEnvironmentVariable("ExampleSettings__Nested__Value") == "this is a nested value"
);

Console.WriteLine(
    "value: {0}, equals: {1}",
    env["ExampleSettings__Nested__Value"],
    env["ExampleSettings__Nested__Value"] == "this is a nested value"
);
