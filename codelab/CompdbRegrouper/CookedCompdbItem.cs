// <copyright file="CookedCompdbItem.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace CompdbRegrouper
{
    public class CookedCompdbItem
    {
        public CookedCompdbItem(CompdbItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this.SourceFile = item.File;
            this.Directory = item.Directory;
            this.Arguments = new List<string>(item.Arguments);
            this.Arguments.RemoveAt(0);

            for (int i = 0; i < this.Arguments.Count; i++)
            {
                if (this.Arguments[i].StartsWith("-I", StringComparison.InvariantCulture))
                {
                    if (this.Arguments[i].Substring(2).StartsWith(item.Directory, StringComparison.InvariantCulture))
                    {
                        this.Arguments[i] = "-I." + this.Arguments[i].Substring(2 + item.Directory.Length);
                    }
                }
                else if (this.Arguments[i] == "-o")
                {
                    this.Arguments.RemoveAt(i);
                    this.DestinationFile = this.Arguments[i];
                    this.Arguments.RemoveAt(i);
                    i--;
                }
                else if (this.Arguments[i] == item.File)
                {
                    this.Arguments.RemoveAt(i);
                    i--;
                }
            }

            this.Arguments = this.Arguments.ToImmutableSortedSet();
        }

        public string SourceFile { get; }

        public string DestinationFile { get; }

        public IList<string> Arguments { get; }

        public string Directory { get; }
    }
}
