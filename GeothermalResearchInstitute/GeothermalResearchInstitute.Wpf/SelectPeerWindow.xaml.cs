// <copyright file="SelectPeerWindow.xaml.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Windows;

namespace GeothermalResearchInstitute.Wpf
{
    /// <summary>
    /// SelectPeerWindow.xaml 的交互逻辑.
    /// </summary>
    public partial class SelectPeerWindow : Window
    {
        public SelectPeerWindow()
        {
            this.InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
