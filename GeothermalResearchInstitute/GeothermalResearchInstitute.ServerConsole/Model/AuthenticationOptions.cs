// <copyright file="AuthenticationOptions.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace GeothermalResearchInstitute.ServerConsole.Model
{
    internal class AuthenticationOptions
    {
        public IList<Credential> Credentials { get; set; }
    }
}
