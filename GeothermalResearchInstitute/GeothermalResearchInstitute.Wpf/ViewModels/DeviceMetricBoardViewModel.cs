// <copyright file="DeviceMetricBoardViewModel.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Windows.Threading;
using GeothermalResearchInstitute.v2;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Prism.Mvvm;
using Prism.Regions;

namespace GeothermalResearchInstitute.Wpf.ViewModels
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance", "CA1822", Justification = "ViewModel.")]
    public class DeviceMetricBoardViewModel : BindableBase, IRegionMemberLifetime, INavigationAware
    {
        private readonly DeviceService.DeviceServiceClient client;
        private readonly ILogger<DeviceMetricBoardViewModel> logger;
        private readonly DispatcherTimer timer;
        private ViewModelContext viewModelContext;
        private Metric metric;

        public DeviceMetricBoardViewModel(
            DeviceService.DeviceServiceClient client,
            ILogger<DeviceMetricBoardViewModel> logger)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.timer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.DataBind, this.Timer_Tick, Dispatcher.CurrentDispatcher);
        }

        public ViewModelContext ViewModelContext
        {
            get => this.viewModelContext;
            set => this.SetProperty(ref this.viewModelContext, value);
        }

        public bool KeepAlive => false;

        public Metric Metric
        {
            get => this.metric;
            set
            {
                this.SetProperty(ref this.metric, value);
                this.RaisePropertyChanged(nameof(this.HeaterPowerKilowatt));
                this.RaisePropertyChanged(nameof(this.OutputWaterPressureMeter));
                this.RaisePropertyChanged(nameof(this.OutputWaterCelsiusDegree));
                this.RaisePropertyChanged(nameof(this.WaterPumpFlowRateCubicMeterPerHour));
                this.RaisePropertyChanged(nameof(this.GroundHeatExchangeKilowatt));
                this.RaisePropertyChanged(nameof(this.InputWaterPressureMeter));
                this.RaisePropertyChanged(nameof(this.InputWaterCelsiusDegree));
                this.RaisePropertyChanged(nameof(this.EnvironmentCelsiusDegree));
                this.RaisePropertyChanged(nameof(this.CompressorHeatExchangeKilowatt));
                this.RaisePropertyChanged(nameof(this.DeltaWaterPressureMeter));
                this.RaisePropertyChanged(nameof(this.HeaterOutputWaterCelsiusDegree));
            }
        }

        public float HeaterPowerKilowatt => this.Metric?.HeaterPowerKilowatt ?? 0;

        public float OutputWaterPressureMeter => this.Metric?.OutputWaterPressureMeter ?? 0;

        public float OutputWaterCelsiusDegree => this.Metric?.OutputWaterCelsiusDegree ?? 0;

        public float WaterPumpFlowRateCubicMeterPerHour => this.Metric?.WaterPumpFlowRateCubicMeterPerHour ?? 0;

        public float GroundHeatExchangeKilowatt =>
            1.167F * this.WaterPumpFlowRateCubicMeterPerHour * MathF.Abs(
                this.InputWaterCelsiusDegree - this.OutputWaterCelsiusDegree);

        public float InputWaterPressureMeter => this.Metric?.InputWaterPressureMeter ?? 0;

        public float InputWaterCelsiusDegree => this.Metric?.InputWaterCelsiusDegree ?? 0;

        public float EnvironmentCelsiusDegree => this.Metric?.EnvironmentCelsiusDegree ?? 0;

        public float CompressorHeatExchangeKilowatt =>
            1.167F * this.WaterPumpFlowRateCubicMeterPerHour * MathF.Abs(
                this.OutputWaterCelsiusDegree - this.HeaterOutputWaterCelsiusDegree);

        public float DeltaWaterPressureMeter => MathF.Abs(this.InputWaterPressureMeter - this.OutputWaterPressureMeter);

        public float HeaterOutputWaterCelsiusDegree => this.Metric?.HeaterOutputWaterCelsiusDegree ?? 0;

        public bool IsNavigationTarget(NavigationContext navigationContext) => false;

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            this.timer.Stop();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.timer.Start();
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                this.Metric = await this.client.GetMetricAsync(
                    new GetMetricRequest
                    {
                        DeviceId = this.ViewModelContext.SelectedDevice.Id,
                    },
                    deadline: DateTime.Now.AddSeconds(1));
            }
            catch (RpcException ex)
            {
                this.logger.LogError(
                    ex,
                    "Failed to get metrics for device {0}",
                    this.ViewModelContext.SelectedDevice.Id);
            }
        }
    }
}
