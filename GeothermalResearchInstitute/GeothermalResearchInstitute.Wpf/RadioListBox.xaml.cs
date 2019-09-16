// <copyright file="RadioListBox.xaml.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GeothermalResearchInstitute.Wpf
{
    /// <summary>
    /// RadioListBox.xaml 的交互逻辑
    /// </summary>
    public partial class RadioListBox : ListBox
    {
        public RadioListBox()
        {
            this.InitializeComponent();

            this.SelectionMode = SelectionMode.Single;
        }

        public new SelectionMode SelectionMode
        {
            get
            {
                return base.SelectionMode;
            }

            private set
            {
                base.SelectionMode = value;
            }
        }

        private bool isTransparent = true;

        public bool IsTransparent
        {
            get
            {
                return this.isTransparent;
            }

            set
            {
                this.isTransparent = value;
                if (value.Equals(true))
                {
                    Border border = this.Template.FindName("theBorder", this) as Border;
                    border.BorderThickness = new Thickness(0.0);
                    border.Background = System.Windows.Media.Brushes.Transparent;
                }
                else
                {
                    Border border = this.Template.FindName("theBorder", this) as Border;
                    border.BorderBrush = this.BorderBrush;
                    border.BorderThickness = this.BorderThickness;
                    border.Background = this.Background;
                }
            }
        }

        private void ItemRadioClick(object sender, RoutedEventArgs e)
        {
            if (e.Source is RadioButton rb)
            {
                ListBoxItem sel = rb.TemplatedParent as ListBoxItem;
                int newIndex = this.ItemContainerGenerator.IndexFromContainer(sel);
                this.SelectedIndex = newIndex;
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            this.CheckRadioButtons(e.RemovedItems, false);
            this.CheckRadioButtons(e.AddedItems, true);
        }

        private void CheckRadioButtons(System.Collections.IList radioButtons, bool isChecked)
        {
            foreach (object item in radioButtons)
            {
                ListBoxItem lbi = this.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;

                if (lbi != null)
                {
                    RadioButton radio = lbi.Template.FindName("radio", lbi) as RadioButton;
                    if (radio != null)
                        radio.IsChecked = isChecked;
                }
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (this.ItemContainerGenerator.ContainerFromIndex(this.SelectedIndex) is ListBoxItem lbi)
            {
                if (lbi.Template.FindName("radio", lbi) is RadioButton radio)
                {
                    radio.IsChecked = true;
                }
            }
        }
    }
}
