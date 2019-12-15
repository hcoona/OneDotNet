// <copyright file="DeviceMetricHistoryViewModel.cs" company="Shuai Zhang">
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
    public class DeviceMetricHistoryViewModel : BindableBase
    {
        private readonly IOptions<CoreOptions> coreOptions;
        private readonly DeviceService.DeviceServiceClient client;
        private ViewModelContext viewModelContext;
        private string nextPageToken = null;
        private bool noMore = false;

        public DeviceMetricHistoryViewModel(
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

        public ObservableCollection<Metric> Metrics { get; } = new ObservableCollection<Metric>();

        public async Task LoadAsync()
        {
            if (this.noMore)
            {
                return;
            }

            var request = new ListMetricsRequest
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
                ListMetricsResponse response = await this.client.ListMetricsAsync(
                    request,
                    deadline: DateTime.UtcNow.AddMilliseconds(this.coreOptions.Value.DefaultReadTimeoutMillis));
                this.Metrics.AddRange(response.Metrics);
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
