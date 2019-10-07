// <copyright file="SelectAllWhenFocusBehavior.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace GeothermalResearchInstitute.Wpf.Views
{
    public static class SelectAllWhenFocusBehavior
    {
        public static readonly DependencyProperty EnableProperty =
                 DependencyProperty.RegisterAttached(
                    "Enable",
                    typeof(bool),
                    typeof(SelectAllWhenFocusBehavior),
                    new FrameworkPropertyMetadata(false, SelectAllWhenFocusBehavior.OnEnableChanged));

        public static bool GetEnable(FrameworkElement frameworkElement)
        {
            if (frameworkElement is null)
            {
                throw new System.ArgumentNullException(nameof(frameworkElement));
            }

            return (bool)frameworkElement.GetValue(EnableProperty);
        }

        public static void SetEnable(FrameworkElement frameworkElement, bool value)
        {
            if (frameworkElement is null)
            {
                throw new System.ArgumentNullException(nameof(frameworkElement));
            }

            frameworkElement.SetValue(EnableProperty, value);
        }

        private static void OnEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FrameworkElement frameworkElement))
            {
                return;
            }

            if (e.NewValue is bool == false)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                frameworkElement.GotFocus += SelectAll;
                frameworkElement.PreviewMouseDown += IgnoreMouseButton;
            }
            else
            {
                frameworkElement.GotFocus -= SelectAll;
                frameworkElement.PreviewMouseDown -= IgnoreMouseButton;
            }
        }

        private static void SelectAll(object sender, RoutedEventArgs e)
        {
            var frameworkElement = e.OriginalSource as FrameworkElement;
            if (frameworkElement is TextBox)
            {
                ((TextBoxBase)frameworkElement).SelectAll();
            }
            else if (frameworkElement is PasswordBox)
            {
                ((PasswordBox)frameworkElement).SelectAll();
            }
        }

        private static void IgnoreMouseButton(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!(sender is FrameworkElement frameworkElement) || frameworkElement.IsKeyboardFocusWithin)
            {
                return;
            }

            e.Handled = true;
            frameworkElement.Focus();
        }
    }
}
