// <copyright file="ProtoUtils.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using GeothermalResearchInstitute.v2;
using GeothermalResearchInstitute.Wpf.Common;

namespace GeothermalResearchInstitute.Wpf.ViewModels
{
    public static class ProtoUtils
    {
        public static string ConvertToString(UserRole role) => role switch
        {
            UserRole.Administrator => Constants.UserRoleAdministrator,
            UserRole.User => Constants.UserRoleOperator,
            _ => throw new ArgumentOutOfRangeException(nameof(role)),
        };
    }
}
