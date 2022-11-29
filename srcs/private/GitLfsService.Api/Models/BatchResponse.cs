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
    public class BatchResponse
    {
        [DataMember(Name = "transfer", IsRequired = true, Order = 1)]
        public string Transfer { get; init; } = default!;

        [DataMember(Name = "objects", IsRequired = true, Order = 2)]
        public List<LfsObject> Objects { get; init; } = new List<LfsObject>();

        [DataContract]
        public record LfsObject
        {
            [DataMember(Name = "oid", IsRequired = true, Order = 1)]
            public string ObjectId { get; init; } = string.Empty;

            [DataMember(Name = "size", IsRequired = true, Order = 2)]
            public long Size { get; init; }

            [DataMember(Name = "authenticated", IsRequired = true, Order = 3)]
            public bool Authenticated { get; init; }

            [DataMember(Name = "actions", IsRequired = true, Order = 4)]
            public Dictionary<string /*actionName*/, BatchActionContent> BatchActions
            { get; init; } = default!;
        }

        [DataContract]
        public record BatchActionContent
        {
            [DataMember(Name = "href", IsRequired = true, Order = 1)]
            public string Link { get; init; } = string.Empty;

            [DataMember(Name = "header", IsRequired = true, Order = 2)]
            public Dictionary<string, string> Headers { get; init; } = default!;

            [DataMember(Name = "expires_in", IsRequired = false, Order = 3)]
            public int? ExpireSeconds { get; init; }
        }
    }
}
