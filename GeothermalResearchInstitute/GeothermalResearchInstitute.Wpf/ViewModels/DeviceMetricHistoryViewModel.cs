// <copyright file="DeviceMetricHistoryViewModel.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GeothermalResearchInstitute.v2;
using Google.Protobuf.WellKnownTypes;
using Prism.Mvvm;

namespace GeothermalResearchInstitute.Wpf.ViewModels
{
    public class DeviceMetricHistoryViewModel : BindableBase
    {
        private readonly DeviceService.DeviceServiceClient client;
        private ViewModelContext viewModelContext;
        private string nextPageToken = null;

        public DeviceMetricHistoryViewModel(DeviceService.DeviceServiceClient client)
        {
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
            var request = new ListMetricsRequest
            {
                DeviceId = this.ViewModelContext.SelectedDevice.Id,
                PageSize = 50,
            };

            if (this.nextPageToken != null)
            {
                request.PageToken = this.nextPageToken;
            }

            ListMetricsResponse response = await this.client.ListMetricsAsync(
                request,
                deadline: DateTime.Now.AddSeconds(5));
            this.Metrics.AddRange(response.Metrics);
            this.nextPageToken = response.NextPageToken;
        }
    }
}
