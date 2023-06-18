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

using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.CommandLine;

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

    //var indexedSourceFiles = source
    //    .EnumerateFiles(
    //        searchPattern: AllFilesSearchPattern,
    //        enumerationOptions: new EnumerationOptions
    //        {
    //            IgnoreInaccessible = false,
    //            MatchType = MatchType.Simple,
    //            RecurseSubdirectories = true,
    //            ReturnSpecialDirectories = false,
    //        })
    //    .Where(file => !file.FullName.Contains('\0'))
    //    .ToImmutableSortedDictionary(
    //        keySelector: file => Path.GetRelativePath(source.FullName, file.FullName),
    //        elementSelector: file => file);

    var indexedSourceFiles = ImmutableDictionary<string, FileInfo>.Empty;
    foreach (var file in source.EnumerateFiles())
    {
        logger.LogInformation("Source file {File} {Valid}", file, !file.FullName.Trim('\0').Contains('\0'));
    }

    foreach (var dir in source.EnumerateDirectories())
    {
        logger.LogInformation("Source directory {Dir} {Valid}", dir, !dir.FullName.Trim('\0').Contains('\0'));
    }

    return Task.CompletedTask;

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
            FileInfo sourceFileInfo = indexedSourceFiles[key];
            FileInfo destinationFileInfo = indexedDestinationFiles[key];

            bool fileEquality = (sourceFileInfo.Length == destinationFileInfo.Length)
                && (sourceFileInfo.LastWriteTimeUtc == destinationFileInfo.LastWriteTimeUtc);
            if (fileEquality)
            {
                logger.LogInformation("== {Key}", key);
            }
            else
            {
                logger.LogInformation("<> {Key}", key);
            }

            return default;
        });

    foreach (var key in indexedSourceFiles.Keys.Except(indexedDestinationFiles.Keys))
    {
        logger.LogInformation("<  {Key}", key);
    }

    foreach (var key in indexedDestinationFiles.Keys.Except(indexedSourceFiles.Keys))
    {
        logger.LogInformation(" > {Key}", key);
    }

    return Task.CompletedTask;
}
