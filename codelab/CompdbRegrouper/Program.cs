// <copyright file="Program.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CompdbRegrouper
{
    internal class Program
    {
        private Program()
        {
        }

        internal static void Main(string[] args)
        {
            string path = args.Length > 0 ? args[0] : "compile_commands.json";

            IList<CompdbItem> compdb;
            using (StreamReader reader = File.OpenText(path))
            {
                compdb = JsonSerializer.Deserialize<List<CompdbItem>>(reader.ReadToEnd(), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });
            }

            IEnumerable<IGrouping<string, CookedCompdbItem>> g = compdb
                .Where(item => !item.Directory.Contains("src/test") && !item.Directory.Contains("src/example"))
                .Select(item => new CookedCompdbItem(item))
                .GroupBy(item => string.Join(",", item.Arguments));

            foreach (IGrouping<string, CookedCompdbItem> p in g)
            {
                Console.WriteLine(p.Key);
                foreach (CookedCompdbItem item in p.OrderBy(i => i.SourceFile))
                {
                    Console.WriteLine("\t{0}", item.SourceFile);
                }
            }
        }
    }
}
