using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GeothermalResearchInstitute.v1;
using GeothermalResearchInstitute.v2;
using GeothermalResearchInstitute.Wpf.Common;
using Grpc.Core;
using Prism.Mvvm;

namespace GeothermalResearchInstitute.Wpf.ViewModels
{
    public class DeviceAlarmHistoryViewModel : BindableBase
    {
        private readonly v2.DeviceService.DeviceServiceClient client;
        private ViewModelContext viewModelContext;
        private string nextPageToken = null;
        private bool noMore = false;

        public DeviceAlarmHistoryViewModel(v2.DeviceService.DeviceServiceClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public ViewModelContext ViewModelContext
        {
            get => this.viewModelContext;
            set => this.SetProperty(ref this.viewModelContext, value);
        }

        public ObservableCollection<AlarmChange> AlarmChanges { get; } = new ObservableCollection<AlarmChange>();

        public async Task LoadAsync()
        {
            if (this.noMore)
            {
                return;
            }

            var request = new ListAlarmChangesRequest
            {
                DeviceId = this.ViewModelContext.SelectedDevice.Id,
                PageSize = 50,
            };

            if (!string.IsNullOrEmpty(this.nextPageToken))
            {
                request.PageToken = this.nextPageToken;
            }

            try
            {
                ListAlarmChangesResponse response = await this.client.ListAlarmChangesAsync(
                    request,
                    deadline: DateTime.UtcNow.AddSeconds(5));
                this.AlarmChanges.AddRange(response.AlarmChanges);
                this.nextPageToken = response.NextPageToken;
            }
            catch (RpcException e)
            {
                e.ShowMessageBox();
            }

            if (string.IsNullOrEmpty(this.nextPageToken))
            {
                this.noMore = true;
            }
        }
    }
}
