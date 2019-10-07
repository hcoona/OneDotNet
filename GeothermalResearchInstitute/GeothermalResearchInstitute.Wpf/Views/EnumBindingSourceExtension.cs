// <copyright file="EnumBindingSourceExtension.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Linq;
using System.Windows.Markup;

namespace GeothermalResearchInstitute.Wpf.Views
{
    public class EnumBindingSourceExtension : MarkupExtension
    {
        private Type enumType;
        private object exclusion;

        public EnumBindingSourceExtension()
        {
        }

        public EnumBindingSourceExtension(Type enumType, object exclusion)
        {
            this.EnumType = enumType;
            this.Exclusion = exclusion;
        }

        public Type EnumType
        {
            get => this.enumType;
            set
            {
                if (value != this.enumType)
                {
                    if (value != null)
                    {
                        Type enumType = Nullable.GetUnderlyingType(value) ?? value;

                        if (!enumType.IsEnum)
                        {
                            throw new ArgumentException("Type must be for an Enum.");
                        }
                    }

                    this.enumType = value;
                }
            }
        }

        public object Exclusion
        {
            get => this.exclusion;
            set
            {
                if (value != this.exclusion)
                {
                    if (value != null && value.GetType() != this.EnumType)
                    {
                        throw new ArgumentException("Exclusion value is not of EnumType.");
                    }

                    this.exclusion = value;
                }
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (this.EnumType == null)
            {
                throw new InvalidOperationException("The EnumType must be specified.");
            }

            Type actualEnumType = Nullable.GetUnderlyingType(this.enumType) ?? this.enumType;
            Array enumValues = Enum.GetValues(actualEnumType)
                .Cast<object>()
                .Where(v => v != this.exclusion)
                .ToArray();

            if (actualEnumType == this.enumType)
            {
                return enumValues;
            }

            Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            enumValues.CopyTo(tempArray, 1);
            return tempArray;
        }
    }
}
