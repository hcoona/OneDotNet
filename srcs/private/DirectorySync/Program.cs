// Copyright (c) 2022 Zhang Shuai<zhangshuai.ustc@gmail.com>.
// All rights reserved.
//
// This file is part of OneDotNet.
//
// OneDotNet is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// OneDotNet is distributed in the hope that it will be useful, but WITHOUT ANY
// WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
// details.
//
// You should have received a copy of the GNU General Public License along with
// OneDotNet. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Immutable;
using System.CommandLine;
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.ClearProviders();
    builder
        .SetMinimumLevel(LogLevel.Debug)
        .AddSimpleConsole(options =>
        {
            options.SingleLine = true;
        });
});

var sourceOption = new Option<DirectoryInfo>(
    name: "--source",
    description: "The source directory.")
{
    Arity = ArgumentArity.ExactlyOne,
    IsRequired = true,
};
var destinationOption = new Option<DirectoryInfo>(
    name: "--destination",
    description: "The destination directory.")
{
    Arity = ArgumentArity.ExactlyOne,
    IsRequired = true,
};
var dryRunOption = new Option<bool>(
    name: "--dry-run",
    getDefaultValue: () => false,
    description: "Do not actually copy/delete files.")
{
    Arity = ArgumentArity.Zero,
};

var rootCommand = new RootCommand("Sync directory from source to destination.");
rootCommand.AddOption(sourceOption);
rootCommand.AddOption(destinationOption);
rootCommand.AddOption(dryRunOption);

rootCommand.SetHandler(
    DoSync,
    sourceOption,
    destinationOption,
    dryRunOption);

return await rootCommand.InvokeAsync(args);

Task DoSync(DirectoryInfo source, DirectoryInfo destination, bool dryRun)
{
    const string AllFilesSearchPattern = "*";

    var logger = loggerFactory.CreateLogger<Program>();

    logger.LogDebug(
        "Source={Source} Destination={Destination} DryRun={DryRun}",
        source,
        destination,
        dryRun);

    var indexedSourceFiles = source
        .EnumerateFiles(
            searchPattern: AllFilesSearchPattern,
            enumerationOptions: new EnumerationOptions
            {
                IgnoreInaccessible = false,
                MatchType = MatchType.Simple,
                RecurseSubdirectories = true,
            })
        .ToImmutableSortedDictionary(
            keySelector: file => Path.GetRelativePath(source.FullName, file.FullName),
            elementSelector: file => file);

    var indexedDestinationFiles = destination
        .EnumerateFiles(
            searchPattern: AllFilesSearchPattern,
            enumerationOptions: new EnumerationOptions
            {
                IgnoreInaccessible = false,
                MatchType = MatchType.Simple,
                RecurseSubdirectories = true,
            })
        .ToImmutableSortedDictionary(
            keySelector: file => Path.GetRelativePath(destination.FullName, file.FullName),
            elementSelector: file => file);

    var cancellationTokenSource = new CancellationTokenSource();

    var processEqualFilesTask = Parallel.ForEachAsync(
        indexedSourceFiles.Keys.Intersect(indexedDestinationFiles.Keys),
        cancellationTokenSource.Token,
        (key, cancellationToken) =>
        {
            return default;
        });

    throw new NotImplementedException();
}
