// <copyright file="Program.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using GeothermalResearchInstitute.v1;
using Grpc.Core;

namespace GeothermalResearchInstitute.ServerConsole
{
    internal class Program
    {
        private const int Port = 50051;

        private static void Main(string[] args)
        {
            Server server = new Server
            {
                Services = {
                    AuthenticationService.BindService(new AuthenticationServiceImpl()),
                    DeviceService.BindService(new DeviceServiceImpl()),
                },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) },
            };
            server.Start();

            Console.WriteLine("DeviceService server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
