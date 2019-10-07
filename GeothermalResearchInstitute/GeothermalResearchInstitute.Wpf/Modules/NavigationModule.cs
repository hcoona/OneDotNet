// <copyright file="NavigationModule.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using GeothermalResearchInstitute.Wpf.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace GeothermalResearchInstitute.Wpf.Modules
{
    public class NavigationModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationView>();
            containerRegistry.RegisterForNavigation<DeviceControlView>();
            containerRegistry.RegisterForNavigation<DeviceWorkingModeView>();
            containerRegistry.RegisterForNavigation<DeviceRunningParameterView>();
            containerRegistry.RegisterForNavigation<DeviceMetricHistoryView>();
        }
    }
}
