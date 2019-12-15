// <copyright file="DeviceAlarmHistoryViewModel.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GeothermalResearchInstitute.v2;
using GeothermalResearchInstitute.Wpf.Common;
using GeothermalResearchInstitute.Wpf.Options;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Prism.Mvvm;

namespace GeothermalResearchInstitute.Wpf.ViewModels
{
    public class DeviceAlarmHistoryViewModel : BindableBase
    {
        private readonly IOptions<CoreOptions> coreOptions;
        private readonly DeviceService.DeviceServiceClient client;
        private ViewModelContext viewModelContext;
        private string nextPageToken = null;
        private bool noMore = false;

        public DeviceAlarmHistoryViewModel(
            IOptions<CoreOptions> coreOptions,
            DeviceService.DeviceServiceClient client)
        {
            this.coreOptions = coreOptions ?? throw new ArgumentNullException(nameof(coreOptions));
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
                PageSize = this.coreOptions.Value.DefaultPageSize,
            };

            if (!string.IsNullOrEmpty(this.nextPageToken))
            {
                request.PageToken = this.nextPageToken;
            }

            try
            {
                ListAlarmChangesResponse response = await this.client.ListAlarmChangesAsync(
                    request,
                    deadline: DateTime.UtcNow.AddMilliseconds(this.coreOptions.Value.DefaultReadTimeoutMillis));
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
