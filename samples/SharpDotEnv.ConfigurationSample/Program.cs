﻿// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder().AddDotEnvFile(".env-sample").Build();

var settings = config.GetSection("ExampleSettings").Get<Sample.ExampleSettings>()!;

Console.WriteLine(
    "value: {0}, equals: {1}",
    config["EXAMPLE_VALUE"],
    config["EXAMPLE_VALUE"] == "this is only a test"
);

Console.WriteLine(
    "value: {0}, equals: {1}",
    config["ExampleSettings:Nested:Value"],
    config["ExampleSettings:Nested:Value"] == "this is a nested value"
);

Console.WriteLine(
    "value: {0}, equals: {1}",
    settings.Nested.Value,
    settings.Nested.Value == "this is a nested value"
);

namespace Sample
{
    public class ExampleSettings
    {
        public class NestedSettings
        {
            public string Value { get; init; } = "";
        }

        public NestedSettings Nested { get; init; } = new();
    }
}
