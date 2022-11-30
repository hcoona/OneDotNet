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
                  select new
                  {
                      Name = g.Key.Name,
                      WordClass = g.Key.WordClass,
                      Senses = string.Join(
                          "<br/>",
                          g
                              .OrderBy(s => s.SenseNumber)
                              .Select(s => $"{s.SenseNumber}.&lt;{s.CefrLevel}&gt;{s.Grammar}{s.Labels}{s.ChineseDefinition}")),
                  };
var entries = from wce in wordClassEntries
              group wce by wce.Name into g
              select $"\"{g.Key}\",\"<ul>{string.Join(string.Empty, g.Select(wce => $"<li><p>{wce.WordClass}</p><p>{wce.Senses}</p></li>"))}</ul>\"";
File.WriteAllLines("words.csv", entries, new UTF8Encoding(false));
