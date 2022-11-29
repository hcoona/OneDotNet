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

using GitLfsService.Api.Controllers;
using GitLfsService.Api.Models;

namespace GitLfsService.Api.DataAccesses
{
    public class LocalFileSystemLfsObjectManager : ILfsObjectManager
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly LinkGenerator linkGenerator;

        public LocalFileSystemLfsObjectManager(IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.linkGenerator = linkGenerator;
        }

        public Task<BatchResponse> PrepareUploadObjects(
            ILfsObjectManager.BatchRequest request,
            CancellationToken cancellationToken)
        {
            // TODO(hcoona): Prefer TusIo protocol.
            if (!request.Transfers.Contains("basic"))
            {
                throw new NotImplementedException(
                    $"Not supported transfers({string.Join(',', request.Transfers)}),"
                        + $" expect transfers in (basic).");
            }

            HttpContext httpContext = this.httpContextAccessor.HttpContext ?? throw new InvalidOperationException("Not HttpContext!");

            return Task.FromResult(new BatchResponse
            {
                Transfer = "basic",
                Objects = (from obj in request.Objects
                           select new BatchResponse.LfsObject
                           {
                               ObjectId = obj.ObjectId,
                               Size = obj.Size,
                               Authenticated = true,
                               BatchActions =
                                   new Dictionary<string, BatchResponse.BatchActionContent>()
                                   {
                                       ["upload"] = new BatchResponse.BatchActionContent()
                                       {
                                           Link = this.linkGenerator.GetUriByAction(
                                               httpContext,
                                               nameof(ObjectsController.BasicTransferUpload),
                                               nameof(ObjectsController))!,
                                           Headers = default!,
                                           ExpireSeconds = (int)TimeSpan.FromHours(1).TotalSeconds,
                                       },
                                       ["verify"] = new BatchResponse.BatchActionContent()
                                       {
                                           Link = this.linkGenerator.GetUriByAction(
                                               httpContext,
                                               nameof(ObjectsController.BasicTransferUpload),
                                               nameof(ObjectsController))!,
                                           Headers = default!,
                                           ExpireSeconds = (int)TimeSpan.FromHours(1).TotalSeconds,
                                       },
                                   },
                           }).ToList(),
            });
        }

        public Task<BatchResponse> PrepareDownloadObjects(
            ILfsObjectManager.BatchRequest request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Download is not implemented yet.");
        }
    }
}
