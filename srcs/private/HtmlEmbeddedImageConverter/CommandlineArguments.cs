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

using CommandLine;

namespace HtmlEmbeddedImageConverter
{
    internal class CommandlineArguments
    {
        [Option('i', "input", Required = true, HelpText = "The input HTML file path.")]
        public string InputFile { get; set; } = string.Empty;

        [Option('o', "output", Required = true, HelpText = "The output HTML file path.")]
        public string OutputFile { get; set; } = string.Empty;
    }
}
