// <copyright file="MsTestLogger.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace Microsoft.Extensions.Logging.MSTest
{
    public class MsTestLogger : ILogger
    {
        private const string LoglevelPadding = ": ";

        private static readonly string MessagePadding = new string(
            ' ',
            GetLogLevelString(LogLevel.Information).Length + LoglevelPadding.Length);

        private static readonly string NewLineWithMessagePadding = Environment.NewLine + MessagePadding;

        [ThreadStatic]
        private static StringBuilder logBuilder;

        public MsTestLogger(string name)
        {
            this.Name = name;
        }

        public string Name { get; }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                this.WriteMessage(logLevel, this.Name, eventId.Id, message, exception);
            }
        }

        public virtual void WriteMessage(LogLevel logLevel, string logName, int eventId, string message, Exception exception)
        {
            var logBuilder = MsTestLogger.logBuilder;
            MsTestLogger.logBuilder = null;

            if (logBuilder == null)
            {
                logBuilder = new StringBuilder();
            }

            // Example:
            // INFO: ConsoleApp.Program[10]
            //       Request received
            var logLevelString = GetLogLevelString(logLevel);

            // category and event id
            logBuilder.Append(LoglevelPadding);
            logBuilder.Append(logName);
            logBuilder.Append("[");
            logBuilder.Append(eventId);
            logBuilder.AppendLine("]");

            if (!string.IsNullOrEmpty(message))
            {
                // message
                logBuilder.Append(MessagePadding);

                var len = logBuilder.Length;
                logBuilder.AppendLine(message);
                logBuilder.Replace(Environment.NewLine, NewLineWithMessagePadding, len, message.Length);
            }

            // Example:
            // System.InvalidOperationException
            //    at Namespace.Class.Function() in File:line X
            if (exception != null)
            {
                // exception message
                logBuilder.AppendLine(exception.ToString());
            }

            if (logBuilder.Length > 0)
            {
                var hasLevel = !string.IsNullOrEmpty(logLevelString);

                // Queue log message
                if (hasLevel)
                {
                    Logger.LogMessage(logLevelString + logBuilder.ToString().TrimEnd());
                }
                else
                {
                    Logger.LogMessage(logBuilder.ToString().TrimEnd());
                }
            }

            logBuilder.Clear();
            if (logBuilder.Capacity > 1024)
            {
                logBuilder.Capacity = 1024;
            }

            MsTestLogger.logBuilder = logBuilder;
        }

        private static string GetLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return "trce";
                case LogLevel.Debug:
                    return "dbug";
                case LogLevel.Information:
                    return "info";
                case LogLevel.Warning:
                    return "warn";
                case LogLevel.Error:
                    return "fail";
                case LogLevel.Critical:
                    return "crit";
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }
    }
}
