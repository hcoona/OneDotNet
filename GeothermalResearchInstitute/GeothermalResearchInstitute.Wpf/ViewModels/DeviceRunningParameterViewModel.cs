// <copyright file="DeviceRunningParameterViewModel.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using GeothermalResearchInstitute.v2;
using GeothermalResearchInstitute.Wpf.Common;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Prism.Commands;
using Prism.Mvvm;

namespace GeothermalResearchInstitute.Wpf.ViewModels
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance", "CA1822", Justification = "ViewModel.")]
    public class DeviceRunningParameterViewModel : BindableBase
    {
        private static readonly DeviceFlowRateControlMode[] CandidateDeviceFlowRateControlModes = new[]
        {
            DeviceFlowRateControlMode.VariableFrequency,
            DeviceFlowRateControlMode.WorkFrequency,
        };

        private static readonly WaterPumpWorkingMode[] CandidateWaterPumpWorkingModes = new[]
        {
            WaterPumpWorkingMode.FixedFlowRate,
            WaterPumpWorkingMode.FixedFrequency,
        };

        private readonly DeviceService.DeviceServiceClient client;
        private ViewModelContext viewModelContext;
        private WorkingMode currentWorkingMode;
        private WorkingMode updatingWorkingMode;
        private RunningParameter currentRunningParameter;
        private RunningParameter updatingRunningParameter;

        public DeviceRunningParameterViewModel(DeviceService.DeviceServiceClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.UpdateCommand = new DelegateCommand(this.ExecuteUpdateCommand);
        }

        public ICollection<DeviceFlowRateControlMode> DeviceFlowRateControlModes =>
            CandidateDeviceFlowRateControlModes;

        public ICollection<WaterPumpWorkingMode> WaterPumpWorkingModes =>
            CandidateWaterPumpWorkingModes;

        public ViewModelContext ViewModelContext
        {
            get => this.viewModelContext;
            set => this.SetProperty(ref this.viewModelContext, value);
        }

        public RunningParameter CurrentRunningParameter
        {
            get => this.currentRunningParameter;
            set
            {
                this.SetProperty(ref this.currentRunningParameter, value);
                this.UpdatingRunningParameter = value?.Clone();

                this.RaisePropertyChanged(nameof(this.CurrentSummerHeaterCelsiusDegree));
                this.RaisePropertyChanged(nameof(this.CurrentWinterHeaterCelsiusDegree));
                this.RaisePropertyChanged(nameof(this.CurrentColdPowerKilowatt));
                this.RaisePropertyChanged(nameof(this.CurrentWarmPowerKilowatt));
                this.RaisePropertyChanged(nameof(this.CurrentWaterPumpFlowRateCubicMeterPerHour));
                this.RaisePropertyChanged(nameof(this.CurrentWaterPumpFrequencyHertz));
            }
        }

        public RunningParameter UpdatingRunningParameter
        {
            get => this.updatingRunningParameter ?? this.CurrentRunningParameter;
            set
            {
                this.SetProperty(ref this.updatingRunningParameter, value);

                this.RaisePropertyChanged(nameof(this.UpdatingSummerHeaterCelsiusDegree));
                this.RaisePropertyChanged(nameof(this.UpdatingWinterHeaterCelsiusDegree));
                this.RaisePropertyChanged(nameof(this.UpdatingColdPowerKilowatt));
                this.RaisePropertyChanged(nameof(this.UpdatingWarmPowerKilowatt));
                this.RaisePropertyChanged(nameof(this.UpdatingWaterPumpFlowRateCubicMeterPerHour));
                this.RaisePropertyChanged(nameof(this.UpdatingWaterPumpFrequencyHertz));
            }
        }

        public WorkingMode CurrentWorkingMode
        {
            get => this.currentWorkingMode;
            set
            {
                this.SetProperty(ref this.currentWorkingMode, value);
                this.UpdatingWorkingMode = value?.Clone();

                this.RaisePropertyChanged(nameof(this.CurrentFlowRateControlMode));
                this.RaisePropertyChanged(nameof(this.CurrentWaterPumpWorkingMode));
                this.RaisePropertyChanged(nameof(this.CurrentWaterPumpFlowRateCubicMeterPerHourVisibility));
                this.RaisePropertyChanged(nameof(this.CurrentWaterPumpFrequencyHertzVisibility));
            }
        }

        public WorkingMode UpdatingWorkingMode
        {
            get => this.updatingWorkingMode ?? this.CurrentWorkingMode;
            set
            {
                this.SetProperty(ref this.updatingWorkingMode, value);

                this.RaisePropertyChanged(nameof(this.UpdatingFlowRateControlMode));
                this.RaisePropertyChanged(nameof(this.UpdatingWaterPumpWorkingMode));
            }
        }

        public float CurrentSummerHeaterCelsiusDegree =>
            this.CurrentRunningParameter?.SummerHeaterCelsiusDegree ?? 0;

        public float UpdatingSummerHeaterCelsiusDegree
        {
            get => this.UpdatingRunningParameter?.SummerHeaterCelsiusDegree ?? 0;
            set
            {
                this.UpdatingRunningParameter.SummerHeaterCelsiusDegree = value;
                this.RaisePropertyChanged();
            }
        }

        public float CurrentWinterHeaterCelsiusDegree =>
            this.CurrentRunningParameter?.WinterHeaterCelsiusDegree ?? 0;

        public float UpdatingWinterHeaterCelsiusDegree
        {
            get => this.UpdatingRunningParameter?.WinterHeaterCelsiusDegree ?? 0;
            set
            {
                this.UpdatingRunningParameter.WinterHeaterCelsiusDegree = value;
                this.RaisePropertyChanged();
            }
        }

        public float CurrentColdPowerKilowatt =>
            this.CurrentRunningParameter?.ColdPowerKilowatt ?? 0;

        public float UpdatingColdPowerKilowatt
        {
            get => this.UpdatingRunningParameter?.ColdPowerKilowatt ?? 0;
            set
            {
                this.UpdatingRunningParameter.ColdPowerKilowatt = value;
                this.RaisePropertyChanged();
            }
        }

        public float CurrentWarmPowerKilowatt =>
            this.CurrentRunningParameter?.WarmPowerKilowatt ?? 0;

        public float UpdatingWarmPowerKilowatt
        {
            get => this.UpdatingRunningParameter?.WarmPowerKilowatt ?? 0;
            set
            {
                this.UpdatingRunningParameter.WarmPowerKilowatt = value;
                this.RaisePropertyChanged();
            }
        }

        public float CurrentWaterPumpFlowRateCubicMeterPerHour =>
            this.CurrentRunningParameter?.WaterPumpFlowRateCubicMeterPerHour ?? 0;

        public float UpdatingWaterPumpFlowRateCubicMeterPerHour
        {
            get => this.UpdatingRunningParameter?.WaterPumpFlowRateCubicMeterPerHour ?? 0;
            set
            {
                this.UpdatingRunningParameter.WaterPumpFlowRateCubicMeterPerHour = value;
                this.RaisePropertyChanged();
            }
        }

        public float CurrentWaterPumpFrequencyHertz =>
            this.CurrentRunningParameter?.WaterPumpFrequencyHertz ?? 0;

        public float UpdatingWaterPumpFrequencyHertz
        {
            get => this.UpdatingRunningParameter?.WaterPumpFrequencyHertz ?? 0;
            set
            {
                this.UpdatingRunningParameter.WaterPumpFrequencyHertz = value;
                this.RaisePropertyChanged();
            }
        }

        public DeviceFlowRateControlMode CurrentFlowRateControlMode =>
            this.CurrentWorkingMode?.DeviceFlowRateControlMode ?? DeviceFlowRateControlMode.VariableFrequency;

        public DeviceFlowRateControlMode UpdatingFlowRateControlMode
        {
            get => this.UpdatingWorkingMode?.DeviceFlowRateControlMode ?? DeviceFlowRateControlMode.VariableFrequency;
            set
            {
                this.UpdatingWorkingMode.DeviceFlowRateControlMode = value;
                this.RaisePropertyChanged();
            }
        }

        public WaterPumpWorkingMode CurrentWaterPumpWorkingMode =>
            this.CurrentWorkingMode?.WaterPumpWorkingMode ?? WaterPumpWorkingMode.FixedFlowRate;

        public WaterPumpWorkingMode UpdatingWaterPumpWorkingMode
        {
            get => this.UpdatingWorkingMode?.WaterPumpWorkingMode ?? WaterPumpWorkingMode.FixedFlowRate;
            set
            {
                this.UpdatingWorkingMode.WaterPumpWorkingMode = value;
                this.RaisePropertyChanged();
            }
        }

        public Visibility CurrentWaterPumpFlowRateCubicMeterPerHourVisibility =>
            this.CurrentWaterPumpWorkingMode == WaterPumpWorkingMode.FixedFlowRate
            ? Visibility.Visible
            : Visibility.Collapsed;

        public Visibility CurrentWaterPumpFrequencyHertzVisibility =>
            this.CurrentWaterPumpWorkingMode == WaterPumpWorkingMode.FixedFrequency
            ? Visibility.Visible
            : Visibility.Collapsed;

        public DelegateCommand UpdateCommand { get; }

        public async Task LoadAsync()
        {
            try
            {
                this.CurrentRunningParameter = await this.client.GetRunningParameterAsync(
                    new GetRunningParameterRequest
                    {
                        DeviceId = this.ViewModelContext.SelectedDevice.Id,
                    },
                    deadline: DateTime.UtcNow.AddMilliseconds(1500));
                this.UpdatingRunningParameter = this.CurrentRunningParameter.Clone();

                this.CurrentWorkingMode = await this.client.GetWorkingModeAsync(
                    new GetWorkingModeRequest
                    {
                        DeviceId = this.ViewModelContext.SelectedDevice.Id,
                    },
                    deadline: DateTime.UtcNow.AddSeconds(5));
                this.UpdatingWorkingMode = this.CurrentWorkingMode.Clone();
            }
            catch (RpcException e)
            {
                e.ShowMessageBox();
            }
        }

        private async void ExecuteUpdateCommand()
        {
            try
            {
                this.CurrentRunningParameter = await this.client.UpdateRunningParameterAsync(
                    new UpdateRunningParameterRequest
                    {
                        DeviceId = this.ViewModelContext.SelectedDevice.Id,
                        RunningParameter = this.UpdatingRunningParameter,
                    },
                    deadline: DateTime.UtcNow.AddMilliseconds(5000));
                this.UpdatingRunningParameter = this.CurrentRunningParameter.Clone();

                this.CurrentWorkingMode = await this.client.UpdateWorkingModeAsync(
                    new UpdateWorkingModeRequest
                    {
                        DeviceId = this.ViewModelContext.SelectedDevice.Id,
                        WorkingMode = this.UpdatingWorkingMode,
                        UpdateMask = FieldMask.FromStringEnumerable<WorkingMode>(new[]
                        {
                            "device_flow_rate_control_mode",
                            "water_pump_working_mode",
                        }),
                    },
                    deadline: DateTime.UtcNow.AddSeconds(5));
                this.UpdatingWorkingMode = this.CurrentWorkingMode.Clone();
            }
            catch (RpcException e)
            {
                e.ShowMessageBox();
            }
        }
    }
}
