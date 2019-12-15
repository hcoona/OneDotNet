// <copyright file="DeviceMetricHistoryExportViewModel.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeothermalResearchInstitute.v2;
using GeothermalResearchInstitute.Wpf.Common;
using GeothermalResearchInstitute.Wpf.Options;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prism.Commands;
using Prism.Mvvm;

namespace GeothermalResearchInstitute.Wpf.ViewModels
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance", "CA1822", Justification = "ViewModel.")]
    public class DeviceMetricHistoryExportViewModel : BindableBase
    {
        private static readonly TimeSpan[] CandidateExportTimeSpans = new TimeSpan[]
        {
            TimeSpan.FromSeconds(10),
        };

        private readonly ILogger<DeviceMetricHistoryExportViewModel> logger;
        private readonly IOptions<CoreOptions> coreOptions;
        private readonly DeviceService.DeviceServiceClient client;
        private ViewModelContext viewModelContext;
        private DateTime startDateTime = DateTime.Now;
        private DateTime endDateTime = DateTime.Now;
        private TimeSpan selectedTimeSpan = CandidateExportTimeSpans[0];

        public DeviceMetricHistoryExportViewModel(
            ILogger<DeviceMetricHistoryExportViewModel> logger,
            IOptions<CoreOptions> coreOptions,
            DeviceService.DeviceServiceClient client)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.coreOptions = coreOptions ?? throw new ArgumentNullException(nameof(coreOptions));
            this.client = client ?? throw new ArgumentNullException(nameof(client));

            this.ExportCommand = new DelegateCommand(this.ExecuteExport, this.CanExport);
        }

        public ICollection<TimeSpan> ExportTimeSpans => CandidateExportTimeSpans;

        public ViewModelContext ViewModelContext
        {
            get => this.viewModelContext;
            set => this.SetProperty(ref this.viewModelContext, value);
        }

        public DateTime StartDateTime
        {
            get => this.startDateTime;
            set => this.SetProperty(ref this.startDateTime, value);
        }

        public DateTime EndDateTime
        {
            get => this.endDateTime;
            set => this.SetProperty(ref this.endDateTime, value);
        }

        public TimeSpan SelectedTimeSpan
        {
            get => this.selectedTimeSpan;
            set => this.SetProperty(ref this.selectedTimeSpan, value);
        }

        public DelegateCommand ExportCommand { get; }

        private bool CanExport() => this.StartDateTime < this.EndDateTime;

        private async void ExecuteExport()
        {
            int errorCounter = 0;
            var metrics = new List<Metric>();
            string nextPageToken = null;
            while (true)
            {
                var request = new ListMetricsRequest
                {
                    DeviceId = this.ViewModelContext.SelectedDevice.Id,
                    StartTime = Timestamp.FromDateTime(this.StartDateTime.ToUniversalTime()),
                    EndTime = Timestamp.FromDateTime(this.EndDateTime.ToUniversalTime()),
                    PageSize = this.coreOptions.Value.DefaultPageSize,
                };

                if (nextPageToken != null)
                {
                    request.PageToken = nextPageToken;
                }

                ListMetricsResponse response;
                try
                {
                    response = await this.client.ListMetricsAsync(
                        request,
                        deadline: DateTime.UtcNow.AddMilliseconds(this.coreOptions.Value.DefaultReadTimeoutMillis));
                }
                catch (RpcException e)
                {
                    this.logger.LogError(
                        e,
                        "Failed to list metrics for device {0}",
                        this.ViewModelContext.SelectedDevice.Id);
                    errorCounter++;
                    if (errorCounter < this.coreOptions.Value.MaxErrorToleranceNum)
                    {
                        continue;
                    }
                    else
                    {
                        e.ShowMessageBox();
                        return;
                    }
                }

                nextPageToken = response.NextPageToken;

                if (response.Metrics.Count == 0)
                {
                    break;
                }

                if (response.Metrics.Any(m => m.CreateTime.ToDateTimeOffset() < this.StartDateTime))
                {
                    metrics.AddRange(response.Metrics.Where(
                        m => m.CreateTime.ToDateTimeOffset() >= this.StartDateTime));
                    break;
                }
                else
                {
                    metrics.AddRange(response.Metrics);
                }

                if (string.IsNullOrEmpty(nextPageToken))
                {
                    break;
                }
            }

            using var saveFileDialog = new SaveFileDialog
            {
                Filter = "逗号分隔文件(*.csv)|*.csv",
                AddExtension = true,
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using var sw = new StreamWriter(
                    File.Open(saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.Read),
                    Encoding.UTF8);

                await sw
                    .WriteLineAsync(
                        "出水温度（摄氏度）,回水温度（摄氏度）,加热器出水温度（摄氏度）,"
                        + "环境温度（摄氏度）,出水压力（米）,回水压力（米）,"
                        + "加热器功率（千瓦）,水泵流量（立方米/小时）")
                    .ConfigureAwait(true);
                foreach (Metric m in metrics)
                {
                    await sw
                        .WriteLineAsync(
                            $"{m.OutputWaterCelsiusDegree:F2},{m.InputWaterCelsiusDegree:F2},"
                            + $"{m.HeaterOutputWaterCelsiusDegree:F2},{m.EnvironmentCelsiusDegree:F2},"
                            + $"{m.OutputWaterPressureMeter:F2},{m.InputWaterPressureMeter:F2},"
                            + $"{m.HeaterPowerKilowatt:F2},{m.WaterPumpFlowRateCubicMeterPerHour:F2}")
                        .ConfigureAwait(true);
                }
            }
        }
    }
}
