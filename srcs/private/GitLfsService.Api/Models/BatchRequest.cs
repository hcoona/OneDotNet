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

using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace GitLfsService.Api.Models
{
    [DataContract]
    public record BatchRequest
    {
        [DataMember(Name = "operator", IsRequired = true, Order = 1)]
        public string Operator { get; init; } = string.Empty;

        [DataMember(Name = "transfers", IsRequired = false, Order = 2)]
        public ImmutableSortedSet<string> Transfers { get; init; } =
            ImmutableSortedSet.Create("basic");

        [DataMember(Name = "objects", IsRequired = true, Order = 4)]
        public List<LfsObject> Objects { get; init; } = new List<LfsObject>();

        [DataMember(Name = "hash_algo", IsRequired = false, Order = 5)]
        public string HashAlgorithm { get; init; } = "sha256";

        [DataContract]
        public record LfsObject
        {
            [DataMember(Name = "oid", IsRequired = true, Order = 1)]
            public string ObjectId { get; init; } = string.Empty;

            [DataMember(Name = "size", IsRequired = true, Order = 2)]
            public long Size { get; init; }
        }
    }
}
