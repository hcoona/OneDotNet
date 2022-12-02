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

using System.IO.Compression;
using System.Text;
using OxfordDictExtractor.GenModel;
using OxfordDictExtractor.ParserModel;

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

foreach (var level in Enum.GetValues<CefrLevel>().Where(l => l != CefrLevel.Unspecified))
{
    // editorconfig-checker-disable
    var wordClassEntries = from word in words
                           from entry in word.Entries
                           where !entry.IsOnlyPhrasalVerb && !entry.IsOnlyIdioms
                           from sense in entry.Senses
                           where !sense.IsXrefOnly
                           where sense.CefrLevel <= level
                              && sense.CefrLevel != CefrLevel.Unspecified
                           group sense by new { entry.Name, entry.WordClass } into g
                           select new WordClassEntry(g.Key.Name, g.Key.WordClass, g.ToList());
    var entries = (from wce in wordClassEntries
                   group wce by wce.Name into g
                   where g.Any(wce => wce.WordSenses.Any(s => s.CefrLevel == level))
                   select new OxfordDictExtractor.GenModel.WordEntry(g.Key, g.ToList())).ToList();

    // editorconfig-checker-enable
    using (var fs = File.Open(
        $"words_{level}.tsv",
        new FileStreamOptions
        {
            Access = FileAccess.Write,
            BufferSize = 4096,
            Mode = FileMode.Create,
            Options = FileOptions.Asynchronous,
            PreallocationSize = 4096 * 1024,
            Share = FileShare.Read,
        }))
    using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
    {
        foreach (var entry in entries)
        {
            await entry.ToStyledDelimitedHtml(sw, delimiter: '\t');
            await sw.WriteLineAsync();
        }
    }

    using (var fs = File.Open(
        $"words_unstyled_{level}.csv",
        new FileStreamOptions
        {
            Access = FileAccess.Write,
            BufferSize = 4096,
            Mode = FileMode.Create,
            Options = FileOptions.Asynchronous,
            PreallocationSize = 4096 * 1024,
            Share = FileShare.Read,
        }))
    using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
    {
        foreach (var entry in entries)
        {
            await entry.ToNoStyledDelimitedHtml(sw, delimiter: ',');
            await sw.WriteLineAsync();
        }
    }
}
