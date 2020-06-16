// <copyright file="CompdbItem.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace CompdbRegrouper
{
    public class CompdbItem
    {
#pragma warning disable CA2227 // 集合属性应为只读
        public IList<string> Arguments { get; set; } = new List<string>();
#pragma warning restore CA2227 // 集合属性应为只读

        public string Directory { get; set; }

        public string File { get; set; }
    }
}
