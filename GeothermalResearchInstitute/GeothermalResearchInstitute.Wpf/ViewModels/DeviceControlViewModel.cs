// <copyright file="DeviceControlViewModel.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using GeothermalResearchInstitute.v2;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace GeothermalResearchInstitute.Wpf.ViewModels
{
    public class DeviceControlViewModel : BindableBase, IRegionMemberLifetime, INavigationAware
    {
        private readonly DeviceService.DeviceServiceClient client;
        private readonly ILogger<DeviceControlViewModel> logger;
        private readonly DispatcherTimer timer;
        private ViewModelContext viewModelContext;
        private Switch switchInfo;
        private Metric metric;

        public DeviceControlViewModel(
            DeviceService.DeviceServiceClient client,
            ILogger<DeviceControlViewModel> logger)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.timer = new DispatcherTimer(
                TimeSpan.FromSeconds(1),
                DispatcherPriority.DataBind,
                this.Timer_Tick,
                Dispatcher.CurrentDispatcher);

            this.SwitchOnClickCommand = new DelegateCommand<string>(this.ExecuteSwitchOnClickCommand);
            this.SwitchOffClickCommand = new DelegateCommand<string>(this.ExecuteSwitchOffClickCommand);
        }

        public DelegateCommand<string> SwitchOnClickCommand { get; }

        public DelegateCommand<string> SwitchOffClickCommand { get; }

        public ViewModelContext ViewModelContext
        {
            get => this.viewModelContext;
            set => this.SetProperty(ref this.viewModelContext, value);
        }

        public Switch Switch
        {
            get => this.switchInfo;
            set => this.SetProperty(ref this.switchInfo, value);
        }

        public Metric Metric
        {
            get => this.metric ?? new Metric();
            set
            {
                this.SetProperty(ref this.metric, value);
                this.RaisePropertyChanged(nameof(this.WaterPump));
                this.RaisePropertyChanged(nameof(this.HeaterWaterTemperature));
            }
        }

        public bool HeaterWaterTemperature
        {
            get => this.Metric.HeaterOutputWaterCelsiusDegree - this.Metric.InputWaterCelsiusDegree > 0;
        }

        public bool WaterPump
        {
            get => this.Metric.WaterPumpFlowRateCubicMeterPerHour > 0;
        }

        public bool KeepAlive => false;

        public bool IsNavigationTarget(NavigationContext navigationContext) => false;

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            this.timer.Stop();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.timer.Start();
        }

        public async Task LoadMetricAsync()
        {
            try
            {
                Metric response = await this.client.GetMetricAsync(
                new GetMetricRequest()
                {
                    DeviceId = this.ViewModelContext.SelectedDevice.Id,
                },
                deadline: DateTime.UtcNow.AddMilliseconds(500));
                this.Metric = response;
            }
            catch (RpcException e)
            {
                this.logger.LogError(
                    e,
                    "Failed to get switch for device {0}",
                    this.ViewModelContext.SelectedDevice.Id);
            }
        }

        public async Task LoadSwitchAsync()
        {
            try
            {
                Switch response = await this.client.GetSwitchAsync(
                    new GetSwitchRequest()
                    {
                        DeviceId = this.ViewModelContext.SelectedDevice.Id,
                    },
                    deadline: DateTime.UtcNow.AddMilliseconds(500));
                this.Switch = response;
            }
            catch (RpcException e)
            {
                this.logger.LogError(
                    e,
                    "Failed to get switch for device {0}",
                    this.ViewModelContext.SelectedDevice.Id);
            }
        }

        private static Switch UpdateSwitchInfo(string type, bool status)
        {
            var obj = new Switch();
            switch (type)
            {
                case "device_power_on":
                    obj.DevicePowerOn = status;
                    break;
                case "exhauster_power_on":
                    obj.ExhausterPowerOn = status;
                    break;
                case "heater_power_on":
                    obj.HeaterPowerOn = status;
                    break;
                case "heater_auto_on":
                    obj.HeaterAutoOn = status;
                    break;
                case "heater_compressor_on":
                    obj.HeaterCompressorOn = status;
                    break;
                case "heater_fan_on":
                    obj.HeaterFanOn = status;
                    break;
                case "heater_four_way_reversing_on":
                    obj.HeaterFourWayReversingOn = status;
                    break;
            }

            return obj;
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            await this.LoadMetricAsync().ConfigureAwait(true);
        }

        private async Task UpdateSwitchAsync(Switch s, FieldMask mask)
        {
            Switch response;
            if (false)
            {
                response = s.Clone();
            }
            else
            {
                response = (await this.client.UpdateSwitchAsync(
                    new UpdateSwitchRequest()
                    {
                        DeviceId = this.ViewModelContext.SelectedDevice.Id,
                        Switch = s.Clone(),
                        UpdateMask = mask,
                    },
                    deadline: DateTime.UtcNow.AddMilliseconds(500))).Clone();
            }

            this.Switch = response;
        }

        private async void ExecuteSwitchOnClickCommand(string type)
        {
            var updateMask = FieldMask.FromString(type);
            Switch obj = UpdateSwitchInfo(type, true);
            await this.UpdateSwitchAsync(obj, updateMask).ConfigureAwait(true);
        }

        private async void ExecuteSwitchOffClickCommand(string type)
        {
            var updateMask = FieldMask.FromString(type);
            Switch obj = UpdateSwitchInfo(type, false);
            await this.UpdateSwitchAsync(obj, updateMask).ConfigureAwait(false);
        }
    }
}
