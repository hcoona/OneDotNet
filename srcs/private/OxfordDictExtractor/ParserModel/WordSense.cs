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

using HtmlAgilityPack;

namespace OxfordDictExtractor
{
    public record WordSense
    {
        // 0: if single sense.
        // positive number if multiple sense.
        public int SenseNumber { get; init; }

        public bool IsXrefOnly { get; init; }

        public bool InOxford3000 { get; init; }

        public bool InOxford5000 { get; init; }

        public CefrLevel CefrLevel { get; init; } = CefrLevel.Unspecified;

        public string? Grammar { get; init; }

        public string? Labels { get; init; }

        public string EnglishDefinition { get; init; } = default!;

        public string? ChineseDefinition { get; init; }

        public List<ExampleEntry> Examples { get; init; } = new();

        public void PrintDebug(TextWriter writer, int nestLevel)
        {
            var indent = new string('\t', nestLevel);
            writer.WriteLine($"{indent}SenseNumber={this.SenseNumber}");
            writer.WriteLine($"{indent}InOxford3000={this.InOxford3000}");
            writer.WriteLine($"{indent}InOxford5000={this.InOxford5000}");
            writer.WriteLine($"{indent}CefrLevel={this.CefrLevel}");
            writer.WriteLine($"{indent}Grammar={this.Grammar}");
            writer.WriteLine($"{indent}Labels={this.Labels}");
            writer.WriteLine($"{indent}EnglishDefinition={this.EnglishDefinition}");
            writer.WriteLine($"{indent}ChineseDefinition={this.ChineseDefinition}");
            writer.WriteLine($"{indent}Examples.Count={this.Examples.Count}");
            writer.WriteLine();
        }

        public static WordSense ParseFromDictContent(HtmlNode liNode)
        {
            var sensetop = liNode.SelectSingleNode("./span[@class='sensetop']");
            if (sensetop.ChildNodes.Count == 1)
            {
                if (sensetop.ChildNodes.Single().GetAttributeValue("class", null) == "xrefs")
                {
                    return new WordSense
                    {
                        IsXrefOnly = true,
                        EnglishDefinition = sensetop.InnerText,
                    };
                }
            }

            var def = liNode.SelectSingleNode(".//span[@class='def']");
            var defChn = liNode.SelectSingleNode(".//defT/chn")
                ?? liNode.SelectSingleNode(".//deft/chn")
                ?? liNode.SelectSingleNode(".//adt/chn");
            var use = liNode.SelectSingleNode(".//span[@class='use']");

            if (def == null)
            {
                var sensetopNextSibling = sensetop.NextSibling;
                while (!sensetopNextSibling.HasClass("xrefs")
                    && !sensetopNextSibling.HasClass("examples")
                    && !sensetopNextSibling.HasClass("collapse"))
                {
                    sensetopNextSibling = sensetopNextSibling.NextSibling;
                }

                if (sensetopNextSibling.HasClass("xrefs"))
                {
                    def = sensetopNextSibling;
                    defChn = sensetopNextSibling;
                }
                else if (def == null && use != null)
                {
                    def = use;
                    defChn = use;
                }
                else
                {
                    throw new InvalidDataException("Cannot find def, xrefs, use node.");
                }
            }

            return new WordSense
            {
                SenseNumber = liNode.GetAttributeValue("sensenum", 0),
                InOxford3000 = liNode.GetAttributeValue("ox3000", "n") == "y"
                    || liNode.GetAttributeValue("fkox3000", "n") == "y",
                InOxford5000 = liNode.GetAttributeValue("ox5000", "n") == "y"
                    || liNode.GetAttributeValue("fkox5000", "n") == "y",
                CefrLevel = liNode
                    .GetAttributes("fkcefr", "cefr")
                    .Where(attr => attr != null)
                    .Select(attr => attr.Value)
                    .SingleOrDefault("unspecified") switch
                {
                    "a1" => CefrLevel.A1,
                    "a2" => CefrLevel.A2,
                    "b1" => CefrLevel.B1,
                    "b2" => CefrLevel.B2,
                    "c1" => CefrLevel.C1,
                    "c2" => CefrLevel.C2,
                    _ => CefrLevel.Unspecified,
                },
                Grammar = liNode.SelectSingleNode(".//span[@class='grammar']")?.InnerText,
                Labels = liNode.SelectSingleNode("./span[@class='labels']")?.InnerText,
                EnglishDefinition = def.InnerText
                    ?? throw new InvalidDataException("Cannot parse def span."),
                ChineseDefinition = defChn?.InnerText
                    ?? throw new InvalidDataException("Cannot parse defT/chn."),
            };
        }

        public record ExampleEntry(string EnglishExample, string ChineseExample);
    }
}
