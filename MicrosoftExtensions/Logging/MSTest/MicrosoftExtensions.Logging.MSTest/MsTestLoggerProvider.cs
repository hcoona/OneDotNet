// <copyright file="MsTestLoggerProvider.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Microsoft.Extensions.Logging.MSTest
{
    public sealed class MsTestLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new MsTestLogger(categoryName);

        public void Dispose()
        {
        }
    }
}
