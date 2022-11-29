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

using System.Runtime.Serialization;

namespace GitLfsService.Api.Models
{
    [DataContract]
    public record ErrorResponse
    {
        [DataMember(Name = "message", IsRequired = true, Order = 1)]
        public string Message { get; init; } = default!;

        [DataMember(Name = "request_id", IsRequired = false, Order = 2)]
        public string? RequestId { get; init; }

        [DataMember(Name = "documentation_url", IsRequired = false, Order = 3)]
        public string? DocumentationUrl { get; init; }
    }
}
