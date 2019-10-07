// <copyright file="UserIdentity.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace PrismLab.Models
{
    public class UserIdentity
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        public string NickName { get; set; }
    }
}
