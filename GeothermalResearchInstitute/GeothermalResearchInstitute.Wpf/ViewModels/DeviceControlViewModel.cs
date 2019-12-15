// <copyright file="DeviceControlViewModel.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using GeothermalResearchInstitute.v2;
using GeothermalResearchInstitute.Wpf.Common;
using GeothermalResearchInstitute.Wpf.Options;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace GeothermalResearchInstitute.Wpf.ViewModels
{
    public class DeviceControlViewModel : BindableBase, IRegionMemberLifetime, INavigationAware
    {
        private readonly ILogger<DeviceControlViewModel> logger;
        private readonly IOptions<CoreOptions> coreOptions;
        private readonly DeviceService.DeviceServiceClient client;
        private readonly DispatcherTimer timer;
        private ViewModelContext viewModelContext;
        private Switch deviceSwitch;
        private Metric metric;

        public DeviceControlViewModel(
            ILogger<DeviceControlViewModel> logger,
            IOptions<CoreOptions> coreOptions,
            DeviceService.DeviceServiceClient client)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.coreOptions = coreOptions ?? throw new ArgumentNullException(nameof(coreOptions));
            this.client = client ?? throw new ArgumentNullException(nameof(client));

            this.timer = new DispatcherTimer(
                TimeSpan.FromMilliseconds(this.coreOptions.Value.DefaultRefreshIntervalMillis),
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
            get => this.deviceSwitch;
            set => this.SetProperty(ref this.deviceSwitch, value);
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
                this.Metric = await this.client.GetMetricAsync(
                    new GetMetricRequest()
                    {
                        DeviceId = this.ViewModelContext.SelectedDevice.Id,
                    },
                    deadline: DateTime.UtcNow.AddMilliseconds(this.coreOptions.Value.DefaultReadTimeoutMillis));
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
                    deadline: DateTime.UtcNow.AddMilliseconds(this.coreOptions.Value.DefaultReadTimeoutMillis));
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

        private async void Timer_Tick(object sender, EventArgs e)
        {
            await this.LoadMetricAsync().ConfigureAwait(true);
        }

        private async void ExecuteSwitchOnClickCommand(string fieldMask)
        {
            var updatingMask = FieldMask.FromString<Switch>(fieldMask);
            var updatingSwitch = new Switch();
            Switch.Descriptor.FindFieldByName(fieldMask).Accessor.SetValue(updatingSwitch, true);
            try
            {
                this.Switch = await this.client.UpdateSwitchAsync(
                        new UpdateSwitchRequest()
                        {
                            DeviceId = this.ViewModelContext.SelectedDevice.Id,
                            Switch = updatingSwitch,
                            UpdateMask = updatingMask,
                        },
                        deadline: DateTime.UtcNow.AddMilliseconds(this.coreOptions.Value.DefaultWriteTimeoutMillis));
            }
            catch (RpcException e)
            {
                e.ShowMessageBox();
            }
        }

        private async void ExecuteSwitchOffClickCommand(string fieldMask)
        {
            var updatingMask = FieldMask.FromString<Switch>(fieldMask);
            var updatingSwitch = new Switch();
            Switch.Descriptor.FindFieldByName(fieldMask).Accessor.SetValue(updatingSwitch, false);
            try
            {
                this.Switch = await this.client.UpdateSwitchAsync(
                    new UpdateSwitchRequest()
                    {
                        DeviceId = this.ViewModelContext.SelectedDevice.Id,
                        Switch = updatingSwitch,
                        UpdateMask = updatingMask,
                    },
                    deadline: DateTime.UtcNow.AddMilliseconds(this.coreOptions.Value.DefaultWriteTimeoutMillis));
            }
            catch (RpcException e)
            {
                e.ShowMessageBox();
            }
        }
    }
}
