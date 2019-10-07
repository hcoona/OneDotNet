// <copyright file="UserBarView.xaml.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel;
using System.Windows.Controls;
using Prism.Common;
using Prism.Regions;
using PrismLab.Models;
using PrismLab.ViewModels;

namespace PrismLab.Views
{
    public partial class UserBarView : UserControl
    {
        public UserBarView()
        {
            this.InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += this.RegionContext_PropertyChanged;
        }

        private void RegionContext_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var userIdentity = (UserIdentity)context.Value;
            ((UserBarViewModel)this.DataContext).UserIdentity = userIdentity;
        }
    }
}
