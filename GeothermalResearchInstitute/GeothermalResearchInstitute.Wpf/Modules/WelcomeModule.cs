// <copyright file="WelcomeModule.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using GeothermalResearchInstitute.Wpf.Common;
using GeothermalResearchInstitute.Wpf.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace GeothermalResearchInstitute.Wpf.Modules
{
    public class WelcomeModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            containerProvider.Resolve<IRegionManager>()
                .RegisterViewWithRegion(Constants.ContentRegion, typeof(WelcomeView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<WelcomeView>();
        }
    }
}
