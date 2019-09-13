// <copyright file="Program.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeothermalResearchInstitute.v1;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GeothermalResearchInstitute.ServerConsole
{
    internal class AuthenticationServiceImpl
        : AuthenticationService.AuthenticationServiceBase
    {
        private readonly ILogger<AuthenticationServiceImpl> logger;
        private readonly IServiceProvider serviceProvider;

        public AuthenticationServiceImpl(
            ILogger<AuthenticationServiceImpl> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;

            List<Credential> credentials = new List<Credential>();
            configuration.GetSection("credentials").Bind(credentials);
            foreach (var c in credentials)
            {
                logger.LogInformation(c.ToString());
            }
        }
        public override Task<AuthenticateResponse> Authenticate(
            AuthenticateRequest request, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
        }
    }
}
