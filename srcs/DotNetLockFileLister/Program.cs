// <copyright file="Program.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using NuGet.Packaging;
using NuGet.ProjectModel;

var database = new ConcurrentDictionary<Tuple<string /* target */, string /* id */>, LockFileDependency>();

foreach (var filename in Directory.EnumerateFiles(args[0], "packages.lock.json", new EnumerationOptions()
{
    MatchType = MatchType.Simple,
    RecurseSubdirectories = true,
}))
{
    var packagesLockFileFormat = PackagesLockFileFormat.Read(filename);
    foreach (var target in packagesLockFileFormat.Targets)
    {
        foreach (var dependency in target.Dependencies)
        {
            if (dependency.Type == PackageDependencyType.Project)
            {
                continue;
            }

            database.AddOrUpdate(Tuple.Create(target.Name, dependency.Id), dependency, (_, exist) =>
            {
                if (exist.Type == PackageDependencyType.Direct)
                {
                    return exist;
                }

                if (dependency.Type == PackageDependencyType.Direct)
                {
                    return dependency;
                }

                if (exist.Type == PackageDependencyType.CentralTransitive)
                {
                    return exist;
                }

                if (exist.ResolvedVersion >= dependency.ResolvedVersion)
                {
                    return exist;
                }

                return dependency;
            });
        }
    }
}

var targetTypeDependencies = (from kvp in database
                              select new
                              {
                                  TargetName = kvp.Key.Item1,
                                  kvp.Value.Type,
                                  Dependency = kvp.Value,
                              }).ToList();

XDocument centralPackageVersionsFile;
using (var reader = XmlReader.Create($"{args[0]}/Directory.Packages.props", new XmlReaderSettings { Async = true }))
{
    centralPackageVersionsFile = await XDocument.LoadAsync(reader, LoadOptions.SetLineInfo, CancellationToken.None);
}

var packageVersions = centralPackageVersionsFile
    .Root
    .ElementsNoNamespace("ItemGroup")
    .SelectMany(itemGroup => itemGroup.ElementsNoNamespace("PackageVersion"))
    .ToDictionary(
        elem => elem.Attribute("Include")?.Value ?? string.Empty,
        elem => elem.Attribute("Version")?.Value);

Console.WriteLine("Can remove these dependencies");
var dependenciesHashSet = targetTypeDependencies.Select(obj => obj.Dependency.Id).ToImmutableHashSet();
foreach (var packageVersion in packageVersions)
{
    if (dependenciesHashSet.Contains(packageVersion.Key))
    {
        continue;
    }

    Console.WriteLine($"\t{packageVersion.Key}");
}

Console.WriteLine();

Console.WriteLine("Suggest pin these dependencies");
var packageVersionsHashSet = packageVersions.Select(packageVersion => packageVersion.Key).ToImmutableHashSet();
foreach (var targetGroup in from ttd in targetTypeDependencies
                            where ttd.Type == PackageDependencyType.Transitive
                               && !ttd.Dependency.Id.StartsWith("runtime.")
                            where !packageVersionsHashSet.Contains(ttd.Dependency.Id)
                            where !ttd.Dependency.Id.Contains(".runtime.")
                            group ttd.Dependency by ttd.TargetName into g
                            orderby g.Key
                            select g)
{
    Console.WriteLine($"\tTargetName={targetGroup.Key}");
    foreach (var dependency in targetGroup.OrderBy(d => d.Id))
    {
        Console.WriteLine($"    <PackageVersion Include=\"{dependency.Id}\" Version=\"{dependency.ResolvedVersion}\" />");
    }
}
