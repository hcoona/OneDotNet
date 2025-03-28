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

using System.Text;
using System.Text.RegularExpressions;
using CommandLine;
using HtmlAgilityPack;
using HtmlEmbeddedImageConverter;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

Parser.Default.ParseArguments<CommandlineArguments>(args)
    .WithParsed(options =>
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.Load(options.InputFile);
        htmlDoc.OptionEmptyCollection = true;

        PngEncoder pngEncoder = new()
        {
            CompressionLevel = PngCompressionLevel.DefaultCompression,
        };

        var imgTags = htmlDoc.DocumentNode.Descendants("img")
            .Where(img => img.Attributes.Contains("src"));

        foreach (var imgTag in imgTags)
        {
            var src = imgTag.GetAttributeValue("src", string.Empty);

            Match match = Constants.WebpRegex().Match(src);
            if (match.Success)
            {
                var base64Data = match.Groups[1].Value;

                byte[] webpBytes = Convert.FromBase64String(base64Data);

                using var image = Image.Load(webpBytes);

                using var pngStream = new MemoryStream();
                image.Save(pngStream, pngEncoder);

                imgTag.SetAttributeValue(
                    "src",
                    $"data:image/png;base64,{Convert.ToBase64String(pngStream.ToArray())}");
            }
        }

        htmlDoc.Save(options.OutputFile, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    })
    .WithNotParsed(errors =>
    {
        Console.Error.WriteLine($"Failed to parse input:");
        foreach (var error in errors)
        {
            Console.Error.WriteLine(error.ToString());
        }
    });
