// <copyright file="App.xaml.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GeothermalResearchInstitute.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public ObservableCollection<string> PeerNodes
        {
            get; set;
        } = new ObservableCollection<string>();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // TODO: change to peer list
            this.PeerNodes.Add("A");
            this.PeerNodes.Add("B");
            this.PeerNodes.Add("C");
        }
    }
}
