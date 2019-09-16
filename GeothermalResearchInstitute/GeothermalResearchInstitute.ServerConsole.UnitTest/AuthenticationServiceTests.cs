// <copyright file="AuthenticationServiceTests.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeothermalResearchInstitute.ServerConsole.UnitTest
{
    [TestClass]
    public class AuthenticationServiceTests
    {
        private IHost Host { get; set; }

        [ClassInitialize]
        public void Init()
        {
            this.Host = new HostBuilder()
                .ConfigureHostConfiguration(builder =>
                {
                    builder.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "Environment", "UnitTest" },
                    });
                })
                .ConfigureAppConfiguration(builder =>
                {

                })
                .ConfigureLogging(builder =>
                {
                })
                .Build();
        }

        [ClassCleanup]
        public void Cleanup()
        {
            this.Host.StopAsync().GetAwaiter().GetResult();
            this.Host.Dispose();
            this.Host = null;
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
