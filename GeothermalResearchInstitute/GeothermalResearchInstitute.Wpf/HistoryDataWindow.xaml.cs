// <copyright file="HistoryDataWindow.xaml.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using OxyPlot;
using OxyPlot.Axes;

namespace GeothermalResearchInstitute.Wpf
{
    /// <summary>
    /// HistoryDataWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HistoryDataWindow : Window
    {
        public HistoryDataWindow()
        {
            this.InitializeComponent();
            this.Points = new List<DataPoint>();

            var random = new Random();
            for (var i = 10; i != 0; i--)
            {
                var dt = DateTime.Now.Subtract(TimeSpan.FromSeconds(i));
                var v = random.NextDouble() * 40;
                this.Points.Add(DateTimeAxis.CreateDataPoint(dt, v));
            }

            this.pltData.InvalidatePlot();

            var timer = new DispatcherTimer(
                TimeSpan.FromSeconds(1), DispatcherPriority.Background,
                (_, __) =>
                {
                    var r = new Random();
                    this.Points.Add(DateTimeAxis.CreateDataPoint(DateTime.Now, r.NextDouble() * 40));
                    this.Points.RemoveAt(0);
                    this.pltData.InvalidatePlot();
                },
                this.Dispatcher);
        }

        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(nameof(Points), typeof(IList<DataPoint>), typeof(Window));

        public IList<DataPoint> Points
        {
            get { return (IList<DataPoint>)this.GetValue(PointsProperty); }
            set { this.SetValue(PointsProperty, value); }
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
