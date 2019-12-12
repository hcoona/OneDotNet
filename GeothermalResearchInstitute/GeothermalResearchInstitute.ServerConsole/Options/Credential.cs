// <copyright file="Credential.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using GeothermalResearchInstitute.v2;

namespace GeothermalResearchInstitute.ServerConsole.Options
{
    public class Credential
    {
        public string Nickname { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public UserRole Role { get; set; }

        public override string ToString()
        {
            return this.Nickname + "(" + this.Username + ":" + this.Role + ")";
        }
    }
}
