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
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1636:File header copyright text should match", Justification = "Reviewed")]
[assembly: SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "<挂起>", Scope = "member", Target = "~F:RateLimiter.SmoothRateLimiter.storedPermits")]
[assembly: SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "<挂起>", Scope = "member", Target = "~F:RateLimiter.SmoothRateLimiter.maxPermits")]
[assembly: SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "<挂起>", Scope = "member", Target = "~F:RateLimiter.SmoothRateLimiter.stableInterval")]
