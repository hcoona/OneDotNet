// <copyright file="GlobalSuppressions.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
//
// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "<挂起>", Scope = "member", Target = "~F:RateLimiter.RateLimiterBase.StopwatchProvider")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "<挂起>", Scope = "member", Target = "~F:RateLimiter.SmoothRateLimiter.storedPermits")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "<挂起>", Scope = "member", Target = "~F:RateLimiter.SmoothRateLimiter.maxPermits")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "<挂起>", Scope = "member", Target = "~F:RateLimiter.SmoothRateLimiter.stableInterval")]
