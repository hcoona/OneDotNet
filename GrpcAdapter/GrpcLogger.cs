// <copyright file="GrpcLogger.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Microsoft.Extensions.Logging;
using ExtensionILogger = Microsoft.Extensions.Logging.ILogger;
using ExtensionILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;
using GrpcILogger = Grpc.Core.Logging.ILogger;

namespace HCOONa.Grpc.MicrosoftExtensions.Logging
{
    public class GrpcLogger : GrpcILogger
    {
        private readonly ExtensionILoggerFactory loggerFactory;
        private readonly ExtensionILogger logger;

        public GrpcLogger(ExtensionILoggerFactory loggerFactory, ExtensionILogger logger)
        {
            this.loggerFactory = loggerFactory;
            this.logger = logger;
        }

        public void Debug(string message)
        {
            this.logger.LogDebug(message);
        }

        public void Debug(string format, params object[] formatArgs)
        {
            this.logger.LogDebug(format, formatArgs);
        }

        public void Error(string message)
        {
            this.logger.LogError(message);
        }

        public void Error(string format, params object[] formatArgs)
        {
            this.logger.LogError(format, formatArgs);
        }

        public void Error(Exception exception, string message)
        {
            this.logger.LogError(exception, message);
        }

        public GrpcILogger ForType<T>()
        {
            return new GrpcLogger(this.loggerFactory, this.loggerFactory.CreateLogger<T>());
        }

        public void Info(string message)
        {
            this.logger.LogInformation(message);
        }

        public void Info(string format, params object[] formatArgs)
        {
            this.logger.LogInformation(format, formatArgs);
        }

        public void Warning(string message)
        {
            this.logger.LogWarning(message);
        }

        public void Warning(string format, params object[] formatArgs)
        {
            this.logger.LogWarning(format, formatArgs);
        }

        public void Warning(Exception exception, string message)
        {
            this.logger.LogWarning(exception, message);
        }
    }
}
