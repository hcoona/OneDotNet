// <copyright file="DeviceControlViewModel.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;
using GeothermalResearchInstitute.v2;
using Google.Protobuf.WellKnownTypes;
using Prism.Commands;
using Prism.Mvvm;

namespace GeothermalResearchInstitute.Wpf.ViewModels
{
    public class DeviceControlViewModel : BindableBase
    {
        private ViewModelContext viewModelContext;
        private Switch switchInfo;
        private Metric metric;
        private DeviceService.DeviceServiceClient client;

        public DeviceControlViewModel(DeviceService.DeviceServiceClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.SwitchOnClickCommand = new DelegateCommand<object>(this.ExecuteSwitchOnClickCommand);
            this.SwitchOffClickCommand = new DelegateCommand<object>(this.ExecuteSwitchOffClickCommand);
        }

        public DelegateCommand<object> SwitchOnClickCommand { get; }

        public DelegateCommand<object> SwitchOffClickCommand { get; }

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
                this.RaisePropertyChanged("WaterPump");
                this.RaisePropertyChanged("HeaterWaterTemperature");
            }
        }

        public bool HeaterWaterTemperature
        {
            get => this.Metric.HeaterOutputWaterCelsiusDegree - this.Metric.InputWaterCelsiusDegree > 0 ? true : false;
        }

        public bool WaterPump
        {
            get => this.Metric.WaterPumpFlowRateCubicMeterPerHour > 0 ? true : false;
        }

        public async Task LoadMetricAsync()
        {
            Metric response = await this.client.GetMetricAsync(
                new GetMetricRequest()
                {
                    DeviceId = this.ViewModelContext.SelectedDevice.Id,
                },
                deadline: DateTime.Now.AddMilliseconds(500));
            this.Metric = response;
        }

        public async Task LoadSwitchAsync()
        {
            Switch response = await this.client.GetSwitchAsync(
                new GetSwitchRequest()
                {
                    DeviceId = this.ViewModelContext.SelectedDevice.Id,
                },
                deadline: DateTime.Now.AddMilliseconds(500));
            this.Switch = response;
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
                    deadline: DateTime.Now.AddMilliseconds(500))).Clone();
            }

            this.Switch = response;
        }

        private async void ExecuteSwitchOnClickCommand(object type)
        {
            FieldMask updateMask = FieldMask.FromString((string)type);
            Switch obj = this.UpdateSwitchInfo((string)type, true);
            await this.UpdateSwitchAsync(obj, updateMask).ConfigureAwait(true);
        }

        private async void ExecuteSwitchOffClickCommand(object type)
        {
            FieldMask updateMask = FieldMask.FromString((string)type);
            Switch obj = this.UpdateSwitchInfo((string)type, false);
            await this.UpdateSwitchAsync(obj, updateMask).ConfigureAwait(false);
        }

        private Switch UpdateSwitchInfo(string type, bool status)
        {
            Switch obj = new Switch();
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
    }
}
