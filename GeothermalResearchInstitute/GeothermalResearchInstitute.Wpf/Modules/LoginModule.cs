// <copyright file="LoginModule.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using GeothermalResearchInstitute.Wpf.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace GeothermalResearchInstitute.Wpf.Modules
{
    public class LoginModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<LoginView>();
        }
    }
}
