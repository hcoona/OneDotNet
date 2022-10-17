// Copyright (c) 2022 Zhang Shuai<zhangshuai.ustc@gmail.com>.
// All rights reserved.
//
// This file is part of OneDotNet.
//
// OneDotNet is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// OneDotNet is distributed in the hope that it will be useful, but WITHOUT ANY
// WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
// details.
//
// You should have received a copy of the GNU General Public License along with
// OneDotNet. If not, see <https://www.gnu.org/licenses/>.

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
