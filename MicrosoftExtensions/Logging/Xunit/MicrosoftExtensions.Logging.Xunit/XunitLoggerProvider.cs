// <copyright file="XunitLoggerProvider.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using Xunit.Abstractions;

namespace Microsoft.Extensions.Logging.Xunit
{
    public sealed class XunitLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper testOutputHelper;

        public XunitLoggerProvider(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        public ILogger CreateLogger(string categoryName) =>
            new XunitLogger(this.testOutputHelper, categoryName);

        public void Dispose()
        {
        }
    }
}
