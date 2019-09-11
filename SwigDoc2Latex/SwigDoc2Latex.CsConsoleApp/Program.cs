// <copyright file="Program.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
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

        static void Main(string[] args)
        {
            var doc = new HtmlDocument();
            doc.DetectEncodingAndLoad("SWIG.html");
            foreach (string line in doc.DocumentNode.SelectSingleNode("//body").ChildNodes.SelectMany(Translate))
            {
                Console.WriteLine(line);
            }
        }

        private static IEnumerable<string> Translate(HtmlNode node)
        {
            switch (node)
            {
                case HtmlCommentNode _:
                    return Enumerable.Empty<string>();
                case HtmlTextNode n:
                    if (!string.IsNullOrWhiteSpace(n.Text))
                    {
                        Console.Error.WriteLine("Drop HtmlTextNode: " + n.Text);
                    }

                    return Enumerable.Empty<string>();
                case HtmlNode n when n.Name == "p":
                    return TranslateParagraph(n);
                case HtmlNode n when n.Name == "div" && n.HasClass("sectiontoc"):
                    return Enumerable.Empty<string>();
                case HtmlNode n when n.Name == "div" && n.Element("pre") != null:
                    return TranslateCode(n);
                case HtmlNode n when HeaderNames.Contains(n.Name):
                    return TranslateHeader(n);
                case HtmlNode n when n.Name == "ul":
                    return Enumerable.Repeat("ul", 1);
                case HtmlNode n when n.Name == "table":
                    return Enumerable.Repeat("table", 1);
                case HtmlNode n when n.Name == "center":
                    return Enumerable.Repeat("center", 1);
                default:
                    throw new NotImplementedException("Unsupported HtmlNode: " + node.Name);
            }
        }

        private static IEnumerable<string> TranslateParagraph(HtmlNode n)
        {
            return n.ChildNodes.Select(TranslateText);
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
            string headerText = TranslateText(NumberingRegex.Replace(anchorNode.InnerText.Trim(), string.Empty, 1));
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

        private static string TranslateText(HtmlNode n)
        {
            switch (n)
            {
                case HtmlTextNode nnn:
                    return TranslateText(nnn.InnerText);
                case HtmlNode nnn when nnn.Name == "tt":
                    return @"\texttt{" + string.Join(" ", nnn.ChildNodes.Select(TranslateText)) + "}";
                case HtmlNode nnn when nnn.Name == "em":
                    return @"\emph{" + string.Join(" ", nnn.ChildNodes.Select(TranslateText)) + "}";
                default:
                    throw new NotImplementedException("Unsupported HtmlNode: " + n.Name);
            }
        }

        private static string TranslateText(string text)
        {
            // TODO(zhangshuai.ustc): Escaping LaTeX special characters.
            return HtmlEntity.DeEntitize(text);
        }
    }
}
