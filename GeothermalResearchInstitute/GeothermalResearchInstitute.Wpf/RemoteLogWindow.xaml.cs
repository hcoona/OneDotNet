// <copyright file="RemoteLogWindow.xaml.cs" company="Shuai Zhang">
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

namespace GeothermalResearchInstitute.Wpf
{
    /// <summary>
    /// RemoteLogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RemoteLogWindow : Window
    {
        public RemoteLogWindow()
        {
            this.InitializeComponent();
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public class LogEntry
        {
            public int Id { get; set; }

            public string Level { get; set; }

            public string Timestamp { get; set; }

            public string Message { get; set; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<LogEntry> logEntries = new List<LogEntry>
            {
                new LogEntry
                {
                    Id = 0,
                    Level = "INFO",
                    Timestamp = "2019-08-03T10:27:00",
                    Message = "这是一条示例消息",
                },
                new LogEntry
                {
                    Id = 1,
                    Level = "WARN",
                    Timestamp = "2019-08-03T10:27:10",
                    Message = "这是一条示例消息",
                },
                new LogEntry
                {
                    Id = 2,
                    Level = "ERROR",
                    Timestamp = "2019-08-03T10:27:20",
                    Message = "这是一条示例消息",
                },
            };
            this.dgLog.ItemsSource = logEntries;
        }
    }
}
