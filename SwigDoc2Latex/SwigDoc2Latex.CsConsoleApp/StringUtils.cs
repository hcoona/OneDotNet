// <copyright file="Program.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace SwigDoc2Latex.CsConsoleApp
{
    public static class StringUtils
    {
        public static string SubstringAfter(string str, string separator)
        {
            int pos = str.IndexOf(separator);
            if (pos == -1)
            {
                return str;
            }
            else
            {
                return str.Substring(pos + 1);
            }
        }
    }
}
