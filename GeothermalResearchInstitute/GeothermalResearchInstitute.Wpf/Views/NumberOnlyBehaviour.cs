// <copyright file="NumberOnlyBehaviour.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GeothermalResearchInstitute.Wpf.Views
{
    /// <summary>
    ///     Exposes attached behaviors that can be
    ///     applied to Control objects.
    /// </summary>
    public static class NumberOnlyBehaviour
    {
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(NumberOnlyBehaviour),
            new UIPropertyMetadata(false, OnValueChanged));

        public static bool GetIsEnabled(Control o)
        {
            if (o is null)
            {
                throw new ArgumentNullException(nameof(o));
            }

            return (bool)o.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(Control o, bool value)
        {
            if (o is null)
            {
                throw new ArgumentNullException(nameof(o));
            }

            o.SetValue(IsEnabledProperty, value);
        }

        private static void OnValueChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            if (!(dependencyObject is Control uiElement))
            {
                return;
            }

            if (e.NewValue is bool && (bool)e.NewValue)
            {
                uiElement.PreviewTextInput += OnTextInput;
                uiElement.PreviewKeyDown += OnPreviewKeyDown;
                DataObject.AddPastingHandler(uiElement, OnPaste);
            }
            else
            {
                uiElement.PreviewTextInput -= OnTextInput;
                uiElement.PreviewKeyDown -= OnPreviewKeyDown;
                DataObject.RemovePastingHandler(uiElement, OnPaste);
            }
        }

        private static void OnTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!e.Text.Any(c => char.IsDigit(c) || c == '.'))
            {
                e.Handled = true;
            }
        }

        private static void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private static void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string text = Convert.ToString(e.DataObject.GetData(DataFormats.Text), CultureInfo.InvariantCulture).Trim();
                if (!text.Any(c => char.IsDigit(c) || c == '.'))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
