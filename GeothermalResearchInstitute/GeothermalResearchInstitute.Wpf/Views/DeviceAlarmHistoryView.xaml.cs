using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GeothermalResearchInstitute.Wpf.ViewModels;
using Prism.Common;
using Prism.Regions;

namespace GeothermalResearchInstitute.Wpf.Views
{
    /// <summary>
    /// DeviceAlarmHistoryView.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceAlarmHistoryView : UserControl
    {
        public DeviceAlarmHistoryView()
        {
            this.InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += this.RegionContext_PropertyChanged;
        }

        private DeviceAlarmHistoryViewModel ViewModel => this.DataContext as DeviceAlarmHistoryViewModel;

        private void RegionContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var viewModelContext = (ViewModelContext)context.Value;
            this.ViewModel.ViewModelContext = viewModelContext;
        }

        private async void DeviceAlarmHistoryView_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModelContext viewModelContext = this.ViewModel.ViewModelContext;
            viewModelContext.UserBarVisibility = Visibility.Visible;
            viewModelContext.BannerVisibility = Visibility.Visible;
            viewModelContext.Title = "故障记录";  // TODO: From resource.

            await this.ViewModel.LoadAsync().ConfigureAwait(true);
        }

        private async void Scroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.OriginalSource is ScrollViewer scrollViewer)
            {
                double verticalOffsetPercentage = e.VerticalOffset / scrollViewer.ScrollableHeight;
                if (verticalOffsetPercentage > 0.75)
                {
                    await this.ViewModel.LoadAsync().ConfigureAwait(true);
                }
            }
        }
    }
}
