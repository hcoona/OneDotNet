// <copyright file="UnitTest1.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Extensions.Logging.Xunit.Tests
{
    public class UnitTest1 : IDisposable
    {
        private readonly LoggerFactory loggerFactory;
        private readonly bool disposed = false;

        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            this.loggerFactory = new LoggerFactory(new[] { new XunitLoggerProvider(testOutputHelper) });
        }

        ~UnitTest1()
        {
            this.Dispose(false);
        }

        [Fact]
        public void Test1()
        {
            var logger = this.loggerFactory.CreateLogger("Test1");
            logger.LogInformation("Hello World!");
            logger = this.loggerFactory.CreateLogger<UnitTest1>();
            logger.LogInformation("Hello World!");
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.loggerFactory.Dispose();
            }
        }
    }
}
