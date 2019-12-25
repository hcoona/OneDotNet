// <copyright file="DeviceMetricHistoryPlotViewModel.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using GeothermalResearchInstitute.v2;
using GeothermalResearchInstitute.Wpf.Common;
using GeothermalResearchInstitute.Wpf.Options;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Commands;
using Prism.Mvvm;

namespace GeothermalResearchInstitute.Wpf.ViewModels
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance", "CA1822", Justification = "ViewModel.")]
    public class DeviceMetricHistoryPlotViewModel : BindableBase
    {
        private static readonly List<MetricFieldDescriptorViewModel> MetricFieldDescriptorViewModels =
            new List<MetricFieldDescriptorViewModel>
            {
                new MetricFieldDescriptorViewModel(
                    "出水温度",
                    0,
                    50,
                    Metric.Descriptor.FindFieldByNumber(Metric.OutputWaterCelsiusDegreeFieldNumber).Accessor),
                new MetricFieldDescriptorViewModel(
                    "回水温度",
                    0,
                    50,
                    Metric.Descriptor.FindFieldByNumber(Metric.InputWaterCelsiusDegreeFieldNumber).Accessor),
                new MetricFieldDescriptorViewModel(
                    "环境温度",
                    0,
                    50,
                    Metric.Descriptor.FindFieldByNumber(Metric.EnvironmentCelsiusDegreeFieldNumber).Accessor),
                new MetricFieldDescriptorViewModel(
                    "加热器功率",
                    0,
                    15,
                    Metric.Descriptor.FindFieldByNumber(Metric.HeaterPowerKilowattFieldNumber).Accessor),
                new MetricFieldDescriptorViewModel(
                    "流量",
                    0,
                    4,
                    Metric.Descriptor.FindFieldByNumber(Metric.WaterPumpFlowRateCubicMeterPerHourFieldNumber).Accessor),
            };

        private readonly ILogger<DeviceMetricHistoryPlotViewModel> logger;
        private readonly IOptions<CoreOptions> coreOptions;
        private readonly DeviceService.DeviceServiceClient client;

        private ViewModelContext viewModelContext;

        private DateTime startDateTime = DateTime.Now.Subtract(TimeSpan.FromDays(1));
        private DateTime endDateTime = DateTime.Now;
        private int intervalMinutes = 10;
        private MetricFieldDescriptorViewModel selectedMetricField;

        private PlotModel plotModel = new PlotModel
        {
            Title = "点击确定生成折线图",
        };

        public DeviceMetricHistoryPlotViewModel(
            ILogger<DeviceMetricHistoryPlotViewModel> logger,
            IOptions<CoreOptions> coreOptions,
            DeviceService.DeviceServiceClient client)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.coreOptions = coreOptions ?? throw new ArgumentNullException(nameof(coreOptions));
            this.client = client ?? throw new ArgumentNullException(nameof(client));

            this.PlotCommand = new DelegateCommand(this.ExecutePlot, this.CanPlot);
        }

        public ViewModelContext ViewModelContext
        {
            get => this.viewModelContext;
            set => this.SetProperty(ref this.viewModelContext, value);
        }

        public DateTime StartDateTime
        {
            get => this.startDateTime;
            set
            {
                this.SetProperty(ref this.startDateTime, value);
                this.PlotCommand.RaiseCanExecuteChanged();
            }
        }

        public DateTime EndDateTime
        {
            get => this.endDateTime;
            set
            {
                this.SetProperty(ref this.endDateTime, value);
                this.PlotCommand.RaiseCanExecuteChanged();
            }
        }

        public int IntervalMinutes
        {
            get => this.intervalMinutes;
            set => this.SetProperty(ref this.intervalMinutes, value);
        }

        public ObservableCollection<MetricFieldDescriptorViewModel> MetricFields { get; } =
            new ObservableCollection<MetricFieldDescriptorViewModel>(MetricFieldDescriptorViewModels);

        public MetricFieldDescriptorViewModel SelectedMetricField
        {
            get => this.selectedMetricField;
            set => this.SetProperty(ref this.selectedMetricField, value);
        }

        public PlotModel PlotModel
        {
            get => this.plotModel;
            set => this.SetProperty(ref this.plotModel, value);
        }

        public DelegateCommand PlotCommand { get; }

        private bool CanPlot() => this.StartDateTime < this.EndDateTime;

        private async void ExecutePlot()
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

            var tmp = new PlotModel { Title = this.SelectedMetricField.DisplayName + "历史数据折线图" };
            tmp.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = DateTimeAxis.ToDouble(this.StartDateTime.ToLocalTime()),
                Maximum = DateTimeAxis.ToDouble(this.EndDateTime.ToLocalTime()),
                StringFormat = "yyyy-MM-dd\nHH:mm:ss",
            });
            tmp.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = this.SelectedMetricField.Minimum,
                Maximum = this.SelectedMetricField.Maximum,
            });

            var series = new LineSeries();

            DateTimeOffset startDateTime = metrics.LastOrDefault()?.CreateTime.ToDateTimeOffset()
                ?? this.StartDateTime;
            var metricInterval = TimeSpan.FromMinutes(this.IntervalMinutes);

            // Notice that the metrics is ordered by create_time descending.
            DateTimeOffset lastKnownMetricCreateTime = DateTimeOffset.MaxValue;

            foreach (Metric m in metrics)
            {
                DateTimeOffset createTimeThreshold = lastKnownMetricCreateTime
                    .Subtract(TimeSpan.FromSeconds(metricInterval.TotalSeconds * 0.9));

                var createTime = m.CreateTime.ToDateTimeOffset();
                if (createTimeThreshold < createTime)
                {
                    continue;
                }

                series.Points.Add(DateTimeAxis.CreateDataPoint(
                    m.CreateTime.ToDateTime().ToLocalTime(),
                    (float)this.SelectedMetricField.Accessor.GetValue(m)));

                lastKnownMetricCreateTime = createTime;
            }

            tmp.Series.Add(series);

            this.PlotModel = tmp;
        }
    }
}
