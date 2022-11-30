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

using System.Collections.Immutable;
using System.IO.Compression;
using System.Net;
using System.Text;
using HtmlAgilityPack;

var document = new HtmlDocument();
using (var fs = File.OpenRead("oxford3000-5000.html"))
using (var sr = new StreamReader(fs, Encoding.UTF8))
{
    document.Load(sr);
}

var words = (from node in document.DocumentNode.SelectNodes(
                "//div[@id='wordlistsContentPanel']/ul/li/a")
             select WebUtility.HtmlDecode(node.GetDirectInnerText()).Trim())
            .ToHashSet();

var specialRules = new Dictionary<string, string>
{
    ["august"] = "August",
    ["id"] = "ID",
    ["it"] = "IT",
    ["march"] = "March",
    ["may"] = "May",
    ["o’clock"] = "o'clock",
};

var dict = new SortedDictionary<string, string>();
using (var fs = ZipFile.OpenRead("dict.tsv.zip").Entries.Single().Open())
using (var sr = new StreamReader(fs))
{
    string? line;
    while ((line = sr.ReadLine()) != null)
    {
        var parts = line.Split('\t', 2, StringSplitOptions.TrimEntries);
        if (words.Contains(parts[0]))
        {
            dict.Add(parts[0], parts[1]);
            continue;
        }

        // Special rules
        if (specialRules.ContainsKey(parts[0]))
        {
            Console.WriteLine($"Hit special rule: {parts[0]} -> {specialRules[parts[0]]}");
            dict.Add(specialRules[parts[0]], parts[1]);
            continue;
        }
    }
}

Console.WriteLine($"Load {dict.Count}/{words.Count} words.");
if (dict.Count != words.Count)
{
    Console.WriteLine($"The missing words are {string.Join(", ", words.Except(dict.Keys))}.");
}

using (var fs = File.OpenWrite("wordlist.tsv"))
using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
{
    foreach (var (key, value) in dict)
    {
        sw.WriteLine($"{key}\t{value}");
    }
}
