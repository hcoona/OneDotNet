// <copyright file="MainFrame.xaml.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Windows;
using System.Windows.Controls;

namespace GeothermalResearchInstitute.Wpf
{
    /// <summary>
    /// MainFrame.xaml 的交互逻辑.
    /// </summary>
    public partial class MainFrame : Page
    {
        private Frame mainFrame;

        public MainFrame()
        {
            this.InitializeComponent();
        }

        public MainFrame(Frame mainFrame)
        {
            this.mainFrame = mainFrame;
        }

        private UserIdentity User
        {
            get { return (UserIdentity)Application.Current.FindResource("User"); }
        }
    }
}
