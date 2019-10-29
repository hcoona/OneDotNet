// <copyright file="HexUtils.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;

namespace TcpServerLab
{
    internal class HexUtils
    {
        private readonly ReadOnlyMemory<byte> bytes;
        private readonly int bytesPerLine;
        private readonly bool showHeader;
        private readonly bool showOffset;
        private readonly bool showAscii;

        private readonly StringBuilder sb = new StringBuilder();

        private int index;

        private HexUtils(ReadOnlyMemory<byte> bytes, int bytesPerLine, bool showHeader, bool showOffset, bool showAscii)
        {
            this.bytes = bytes;
            this.bytesPerLine = bytesPerLine;
            this.showHeader = showHeader;
            this.showOffset = showOffset;
            this.showAscii = showAscii;
        }

        public static string Dump(ReadOnlyMemory<byte> bytes, int bytesPerLine = 16, bool showHeader = true, bool showOffset = true, bool showAscii = true)
        {
            return new HexUtils(bytes, bytesPerLine, showHeader, showOffset, showAscii).Dump();
        }

        private static string Translate(byte b)
        {
            return b < 32 ? "." : Encoding.ASCII.GetString(new[] { b });
        }

        private string Dump()
        {
            if (this.showHeader)
            {
                this.WriteHeader();
            }

            this.WriteBody();
            return this.sb.ToString();
        }

        private void WriteHeader()
        {
            if (this.showOffset)
            {
                this.sb.Append("Offset(h)  ");
            }

            for (int i = 0; i < this.bytesPerLine; i++)
            {
                this.sb.Append($"{i & 0xFF:X2}");
                if (i + 1 < this.bytesPerLine)
                {
                    this.sb.Append(" ");
                }
            }

            this.sb.AppendLine();
        }

        private void WriteBody()
        {
            while (this.index < this.bytes.Length)
            {
                if (this.index % this.bytesPerLine == 0)
                {
                    if (this.index > 0)
                    {
                        if (this.showAscii)
                        {
                            this.WriteAscii();
                        }

                        this.sb.AppendLine();
                    }

                    if (this.showOffset)
                    {
                        this.WriteOffset();
                    }
                }

                this.WriteByte();
                if (this.index % this.bytesPerLine != 0 && this.index < this.bytes.Length)
                {
                    this.sb.Append(" ");
                }
            }

            if (this.showAscii)
            {
                this.WriteAscii();
            }
        }

        private void WriteOffset()
        {
            this.sb.Append($"{this.index:X8}   ");
        }

        private void WriteByte()
        {
            this.sb.Append($"{this.bytes.Span[this.index]:X2}");
            this.index++;
        }

        private void WriteAscii()
        {
            int backtrack = ((this.index - 1) / this.bytesPerLine) * this.bytesPerLine;
            int length = this.index - backtrack;

            // This is to fill up last string of the dump if it's shorter than _bytesPerLine
            this.sb.Append(new string(' ', (this.bytesPerLine - length) * 3));

            this.sb.Append("   ");
            for (int i = 0; i < length; i++)
            {
                this.sb.Append(Translate(this.bytes.Span[backtrack + i]));
            }
        }
    }
}
