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
using OxfordDictExtractor;
using OxfordDictExtractor.GenModel;

var words = new List<Word>();
using (var fs = ZipFile.OpenRead("wordlist.tsv.zip").Entries.Single().Open())
using (var sr = new StreamReader(fs))
{
    string? line;
    while ((line = sr.ReadLine()) != null)
    {
        var parts = line.Split('\t', 2, StringSplitOptions.TrimEntries);
        if (parts[1].StartsWith("@@@", StringComparison.InvariantCulture))
        {
            Console.WriteLine("Skip linking word: " + parts[0]);
            continue;
        }

        words.Add(Word.ParseFromDictContent(parts[0], parts[1]));
    }
}

var wordClassEntries = from word in words
                       from entry in word.Entries
                       where !entry.IsOnlyPhrasalVerb && !entry.IsOnlyIdioms
                       from sense in entry.Senses
                       where !sense.IsXrefOnly
                       where sense.CefrLevel == CefrLevel.A1
                       group sense by new { entry.Name, entry.WordClass } into g
                       select new WordClassEntry(g.Key.Name, g.Key.WordClass, g.ToList());
var entries = (from wce in wordClassEntries
               group wce by wce.Name into g
               select new OxfordDictExtractor.GenModel.WordEntry(g.Key, g.ToList())).ToList();

using (var fs = File.OpenWrite("words.tsv"))
using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
{
    foreach (var entry in entries)
    {
        await entry.ToStyledDelimitedHtml(sw, delimiter: '\t');
        await sw.WriteLineAsync();
    }
}

using (var fs = File.OpenWrite("words_unstyled.csv"))
using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
{
    foreach (var entry in entries)
    {
        await entry.ToNoStyledDelimitedHtml(sw, delimiter: ',');
        await sw.WriteLineAsync();
    }
}
