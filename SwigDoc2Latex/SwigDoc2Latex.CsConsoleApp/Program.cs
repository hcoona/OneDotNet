// <copyright file="Program.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace SwigDoc2Latex.CsConsoleApp
{
    internal class Program
    {
        private static readonly ISet<string> HeaderNames = new HashSet<string>
        {
            "h1", "h2", "h3", "h4",
        };

        private static readonly Regex NumberingRegex = new Regex(@"\d+(\.\d+)* ", RegexOptions.Compiled);

        private static readonly Regex LatexReservedCharRegex = new Regex(@"(?<!\\)([#$%^&_{}~\\])", RegexOptions.Compiled);

        static void Main(string[] args)
        {
            // TODO(zhangshuai.ustc): Rewrite HTML document before translation.
            var doc = new HtmlDocument();
            doc.DetectEncodingAndLoad(args[0]);
            using StreamWriter writer = new StreamWriter(File.Open(args[1], FileMode.Create, FileAccess.Write));
            foreach (string line in doc.DocumentNode.SelectSingleNode("//body").ChildNodes.SelectMany(Translate))
            {
                writer.WriteLine(line);
                Console.WriteLine(line);
            }
        }

        private static IEnumerable<string> Translate(HtmlNode node)
        {
            return node switch
            {
                HtmlCommentNode _ => Enumerable.Empty<string>(),
                HtmlTextNode n => TranslateText(n.InnerText),
                HtmlNode n when n.Name == "p" => TranslateParagraph(n),
                HtmlNode n when n.Name == "br" => Enumerable.Empty<string>(),  // TODO(zhangshuai.ds): Convert to definition list.
                HtmlNode n when n.Name == "div" && n.HasClass("sectiontoc") => Enumerable.Empty<string>(),
                HtmlNode n when n.Name == "div" && n.Element("pre") != null => TranslateCode(n),
                HtmlNode n when n.Name == "div" && n.Element("i") != null => Enumerable.Concat(
                    new string[] { @"\begin{quote}" },
                    Enumerable.Concat(
                        n.ChildNodes.SelectMany(Translate),
                        new string[] { @"\end{quote}" })),
                HtmlNode n when HeaderNames.Contains(n.Name) => TranslateHeader(n),
                HtmlNode n when n.Name == "ul" || n.Name == "ol" => TranslateList(n),
                HtmlNode n when n.Name == "li" => Enumerable.Concat(new[] { "\\item" }, n.ChildNodes.SelectMany(Translate)),
                HtmlNode n when n.Name == "table" => TranslateTable(n),
                HtmlNode n when n.Name == "center" => TranslateCentering(n),
                HtmlNode n when n.Name == "tt" =>
                    Enumerable.Repeat(@"\texttt{" + string.Join(" ", n.ChildNodes.SelectMany(Translate)) + "}", 1),
                HtmlNode n when n.Name == "em" =>
                    Enumerable.Repeat(@"\emph{" + string.Join(" ", n.ChildNodes.SelectMany(Translate)) + "}", 1),
                HtmlNode n when n.Name == "b" =>
                    Enumerable.Repeat(@"\textbf{" + string.Join(" ", n.ChildNodes.SelectMany(Translate)) + "}", 1),
                HtmlNode n when n.Name == "i" =>
                    Enumerable.Repeat(@"\textit{" + string.Join(" ", n.ChildNodes.SelectMany(Translate)) + "}", 1),
                HtmlNode n when !n.HasChildNodes => Enumerable.Empty<string>(),
                HtmlNode n when n.Name == "a" =>
                    Enumerable.Repeat(
                        string.Format(
                            @"\hyperref[{0}]{{{1}}}",
                            StringUtils.SubstringAfter(n.Attributes.Single(attr => attr.Name == "href").Value, "#"),
                            string.Join(" ", n.ChildNodes.SelectMany(Translate))),
                        1),
                HtmlNode n when n.Name == "code" =>
                    Enumerable.Repeat(@"\texttt{" + string.Join(" ", n.ChildNodes.SelectMany(Translate)) + "}", 1),
                _ => throw new NotImplementedException("Unsupported HtmlNode: " + node.Name),
            };
        }

        private static IEnumerable<string> TranslateTable(HtmlNode n)
        {
            // TODO(zhangshuai.ustc): Implement it.
            return Enumerable.Empty<string>();
        }

        private static IEnumerable<string> TranslateCentering(HtmlNode n)
        {
            return Enumerable.Concat(
                new string[] { @"\begin{center}" },
                Enumerable.Concat(
                    n.ChildNodes.SelectMany(Translate),
                    new string[] { @"\end{center}" }));
        }

        private static IEnumerable<string> TranslateList(HtmlNode n)
        {
            string env = n.Name switch
            {
                "ul" => "itemize",
                "ol" => "enumerate",
                _ => throw new NotImplementedException("Unsupported list: " + n.Name),
            };
            return Enumerable.Concat(
                new string[] { @"\begin{" + env + "}" },
                Enumerable.Concat(
                    n.ChildNodes.SelectMany(Translate),
                    new string[] { @"\end{" + env + "}" }));
        }

        private static IEnumerable<string> TranslateParagraph(HtmlNode n)
        {
            return n.ChildNodes.SelectMany(Translate).Select(s => s.Trim('\n'));
        }

        private static IEnumerable<string> TranslateCode(HtmlNode n)
        {
            string codeLanguage = n.GetClasses().Single();
            string codeContent = HtmlEntity.DeEntitize(n.Element("pre").InnerText);
            if (codeLanguage == "shell" || codeLanguage == "targetlang")
            {
                return new string[]
                {
                    @"\begin{verbatim}",
                    codeContent.Trim('\n'),
                    @"\end{verbatim}",
                };
            }
            else
            {
                if (codeLanguage == "code")
                {
                    codeLanguage = "swig";
                }

                return new string[]
                {
                    @"\begin{minted}{" + codeLanguage + "}",
                    codeContent.Trim('\n'),
                    @"\end{minted}",
                };
            }
        }

        private static IEnumerable<string> TranslateHeader(HtmlNode n)
        {
            HtmlNode anchorNode = n.Element("a");
            string anchorLabel = anchorNode.Attributes.Single(attr => attr.Name == "name").Value;
            string headerText = string.Join(" ", TranslateText(NumberingRegex.Replace(anchorNode.InnerText.Trim(), string.Empty, 1)));
            var directive = n.Name switch
            {
                "h1" => "chapter",
                "h2" => "section",
                "h3" => "subsection",
                "h4" => "subsubsection",
                _ => throw new NotImplementedException("Unsupported header: " + n.Name),
            };
            return Enumerable.Repeat(
                string.Format("\\{0}{{{1}\\label{{{2}}}}}", directive, headerText, anchorLabel),
                1);
        }

        private static IEnumerable<string> TranslateText(string text)
        {
            return Enumerable.Repeat(LatexEscape(HtmlEntity.DeEntitize(text)), 1);
        }

        private static string LatexEscape(string v)
        {
            return LatexReservedCharRegex.Replace(v, "\\$1");
        }
    }
}
