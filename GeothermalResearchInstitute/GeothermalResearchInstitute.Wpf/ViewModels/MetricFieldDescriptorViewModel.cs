// <copyright file="MetricFieldDescriptorViewModel.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using Google.Protobuf.Reflection;

namespace GeothermalResearchInstitute.Wpf.ViewModels
{
    public class MetricFieldDescriptorViewModel
    {
        public MetricFieldDescriptorViewModel(
            string displayName,
            double minimum,
            double maximum,
            IFieldAccessor accessor)
        {
            this.DisplayName = displayName;
            this.Minimum = minimum;
            this.Maximum = maximum;
            this.Accessor = accessor;
        }

        public string DisplayName { get; }

        public double Minimum { get; }

        public double Maximum { get; }

        public IFieldAccessor Accessor { get; }
    }
}
