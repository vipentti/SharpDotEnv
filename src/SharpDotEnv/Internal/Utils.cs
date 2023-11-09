// Copyright 2023 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpDotEnv.Internal
{
    internal static class Utils
    {
        public static string GetEnvFilePath(
            string envFilePath,
            int levelsToSearch,
            bool ignoreExceptions
        )
        {
            // Absolute paths should be returned as is.
            if (Path.IsPathRooted(envFilePath))
            {
                return envFilePath;
            }

            var searchedDirectories = new List<string>();

            var envFileName = Path.GetFileName(envFilePath) ?? "";
            var envFileDirectory = Path.GetDirectoryName(envFilePath) ?? "";

            var directoriesToSearch = new HashSet<string>()
            {
                PathCombine(AppContext.BaseDirectory, envFileDirectory),
                PathCombine(Directory.GetCurrentDirectory(), envFileDirectory),
            };

            foreach (var directory in directoriesToSearch)
            {
                var foundFile = SearchForFileStartingFrom(directory);

                if (!string.IsNullOrEmpty(foundFile))
                {
                    return foundFile;
                }
            }

            if (!ignoreExceptions)
            {
                throw new FileNotFoundException(
                    $"Env file: '{envFilePath}' not found in:"
                        + $"{Environment.NewLine}  {string.Join(Environment.NewLine + "  ", searchedDirectories)}"
                );
            }

            return "";

            string SearchForFileStartingFrom(string directory)
            {
                var cwd = new DirectoryInfo(directory);

                if (!cwd.Exists)
                {
                    searchedDirectories.Add($"{cwd.FullName} (directory does not exist)");
                    return "";
                }

                int count = levelsToSearch;

                for (; cwd != null && count > 0; count--, cwd = cwd.Parent)
                {
                    searchedDirectories.Add(cwd.FullName);
                    var envFile = cwd.GetFiles(envFileName, SearchOption.TopDirectoryOnly)
                        .FirstOrDefault();

                    if (envFile != null)
                    {
                        return envFile.FullName;
                    }
                }

                return "";
            }
        }

        /// <summary>
        /// Combine paths ensuring the additional paths contain only the correct separators
        /// </summary>
        public static string PathCombine(string basePath, params string[] additional)
        {
            var splits = additional.Select(s => s.Split(pathSplitCharacters)).ToArray();
            var totalLength = splits.Sum(arr => arr.Length);
            var segments = new string[totalLength + 1];
            segments[0] = basePath;
            var i = 0;
            foreach (var split in splits)
            {
                foreach (var value in split)
                {
                    i++;
                    segments[i] = value;
                }
            }
            return Path.Combine(segments);
        }

        private static readonly char[] pathSplitCharacters = new char[] { '/', '\\' };
    }
}
