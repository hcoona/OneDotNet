// <copyright file="ByDesignException.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace TimeLimiterUnitTest
{
    internal class ByDesignException : Exception
    {
    }

#pragma warning disable SA1402 // File may only contain a single type
    internal class ByDesignException<T> : ByDesignException
#pragma warning restore SA1402 // File may only contain a single type
    {
        public ByDesignException(T payload)
        {
            this.Data.Add("payload", payload);
        }

        public T Payload
        {
            get { return (T)this.Data["payload"]; }
        }
    }
}
