// <copyright file="FakeAuthenticationServiceClient.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using GeothermalResearchInstitute.v1;
using Grpc.Core;
using Grpc.Core.Testing;

namespace GeothermalResearchInstitute.Wpf.FakeClients
{
    internal class FakeAuthenticationServiceClient : AuthenticationService.AuthenticationServiceClient
    {
        public override AsyncUnaryCall<AuthenticateResponse> AuthenticateAsync(
            AuthenticateRequest request,
            Metadata headers = null,
            DateTime? deadline = null,
            CancellationToken cancellationToken = default)
        {
            if (request.Username == "user" && request.Password == "user")
            {
                return TestCalls.AsyncUnaryCall(
                    Task.FromResult(
                        new AuthenticateResponse()
                        {
                            Nickname = "用户1",
                            Role = UserRole.User,
                        }),
                    Task.FromResult(new Metadata()),
                    () => Status.DefaultSuccess,
                    () => new Metadata(),
                    () => { });
            }
            else if (request.Username == "admin" && request.Password == "admin")
            {
                return TestCalls.AsyncUnaryCall(
                    Task.FromResult(
                        new AuthenticateResponse()
                        {
                            Nickname = "管理员1",
                            Role = UserRole.Administrator,
                        }),
                    Task.FromResult(new Metadata()),
                    () => Status.DefaultSuccess,
                    () => new Metadata(),
                    () => { });
            }
            else
            {
                return TestCalls.AsyncUnaryCall(
                    Task.FromResult(new AuthenticateResponse()),
                    Task.FromResult(new Metadata()),
                    () => new Status(StatusCode.Unauthenticated, "Invalid username or password."),
                    () => new Metadata(),
                    () => { });
            }
        }
    }
}
