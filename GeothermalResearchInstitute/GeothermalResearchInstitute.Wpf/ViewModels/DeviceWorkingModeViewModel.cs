// <copyright file="DeviceWorkingModeViewModel.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeothermalResearchInstitute.v2;
using GeothermalResearchInstitute.Wpf.Common;
using GeothermalResearchInstitute.Wpf.Options;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Prism.Commands;
using Prism.Mvvm;

namespace GeothermalResearchInstitute.Wpf.ViewModels
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance", "CA1822", Justification = "ViewModel.")]
    public class DeviceWorkingModeViewModel : BindableBase
    {
        private static readonly DeviceWorkingMode[] CandidateDeviceWorkingModes = new[]
        {
            DeviceWorkingMode.TemperatureDetermination,
            DeviceWorkingMode.FixedWarmPower,
            DeviceWorkingMode.FixedColdPower,
            DeviceWorkingMode.SummerSituation,
            DeviceWorkingMode.WinterSituation,
        };

        private readonly IOptions<CoreOptions> coreOptions;
        private readonly DeviceService.DeviceServiceClient client;
        private ViewModelContext viewModelContext;
        private WorkingMode currentWorkingMode;
        private DeviceWorkingMode selectedWorkingMode;

        public DeviceWorkingModeViewModel(
            IOptions<CoreOptions> coreOptions,
            DeviceService.DeviceServiceClient client)
        {
            this.coreOptions = coreOptions ?? throw new ArgumentNullException(nameof(coreOptions));
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.UpdateDeviceWorkingModeCommand = new DelegateCommand(
                this.ExecuteUpdateDeviceWorkingModeCommand, this.CanUpdateDeviceWorkingModeCommand);
        }

        public ViewModelContext ViewModelContext
        {
            get => this.viewModelContext;
            set => this.SetProperty(ref this.viewModelContext, value);
        }

        public ICollection<DeviceWorkingMode> DeviceWorkingModes => CandidateDeviceWorkingModes;

        public WorkingMode CurrentWorkingMode
        {
            get => this.currentWorkingMode;
            set
            {
                this.SetProperty(ref this.currentWorkingMode, value);
                this.RaisePropertyChanged(nameof(this.CurrentDeviceWorkingMode));
                this.RaisePropertyChanged(nameof(this.SelectedDeviceWorkingMode));
                this.UpdateDeviceWorkingModeCommand.RaiseCanExecuteChanged();
            }
        }

        public DeviceWorkingMode CurrentDeviceWorkingMode
        {
            get => this.CurrentWorkingMode?.DeviceWorkingMode ?? DeviceWorkingMode.TemperatureDetermination;
        }

        public DeviceWorkingMode SelectedDeviceWorkingMode
        {
            get => this.selectedWorkingMode == DeviceWorkingMode.Unspecified
                ? this.CurrentDeviceWorkingMode
                : this.selectedWorkingMode;
            set
            {
                this.SetProperty(ref this.selectedWorkingMode, value);
                this.UpdateDeviceWorkingModeCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand UpdateDeviceWorkingModeCommand { get; }

        public async Task LoadWorkingMode()
        {
            try
            {
                this.CurrentWorkingMode = await this.client.GetWorkingModeAsync(
                    new GetWorkingModeRequest
                    {
                        DeviceId = this.ViewModelContext.SelectedDevice.Id,
                    },
                    deadline: DateTime.UtcNow.AddMilliseconds(this.coreOptions.Value.DefaultReadTimeoutMillis));
            }
            catch (RpcException e)
            {
                e.ShowMessageBox();
            }
        }

        private bool CanUpdateDeviceWorkingModeCommand() => this.selectedWorkingMode != DeviceWorkingMode.Unspecified;

        private async void ExecuteUpdateDeviceWorkingModeCommand()
        {
            try
            {
                this.CurrentWorkingMode = await this.client.UpdateWorkingModeAsync(
                    new UpdateWorkingModeRequest
                    {
                        DeviceId = this.ViewModelContext.SelectedDevice.Id,
                        WorkingMode = new WorkingMode
                        {
                            DeviceWorkingMode = this.SelectedDeviceWorkingMode,
                        },
                        UpdateMask = FieldMask.FromString<WorkingMode>("device_working_mode"),
                    },
                    deadline: DateTime.UtcNow.AddMilliseconds(this.coreOptions.Value.DefaultWriteTimeoutMillis));
                this.SelectedDeviceWorkingMode = DeviceWorkingMode.Unspecified;
            }
            catch (RpcException e)
            {
                e.ShowMessageBox();
            }
        }
    }
}
