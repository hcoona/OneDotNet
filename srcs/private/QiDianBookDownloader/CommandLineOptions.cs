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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace QiDianBookDownloader
{
    public record CommandLineOptions
    {
        [Option('v', "verbose", Required = false, HelpText = "Show verbose logs.")]
        public bool Verbose { get; set; }

        [Option('u', "username", Required = true, HelpText = "QiDian username.")]
        public string? UserName { get; set; }

        [Option('p', "password", Required = true, HelpText = "QiDian password.")]
        public string? Password { get; set; }

        [Option("bookId", Required = true, HelpText = "QiDian book ID.")]
        public long? BookId { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output directory.")]
        public string? OutputDirectory { get; set; }
    }
}
