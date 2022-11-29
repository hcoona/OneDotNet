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
using System.Diagnostics;
using GitLfsService.Api.DataAccesses;
using GitLfsService.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace GitLfsService.Api.Controllers
{
    [ApiController]
    [Route("/{org}/{repo}/info/lfs/objects")]
    [Consumes(Constants.GitLfsMediaTypeName)]
    [Produces(Constants.GitLfsMediaTypeName)]
    public class ObjectsController : ControllerBase
    {
        private const string GitLfsBatchApiSpecificationUrl =
            "https://github.com/git-lfs/git-lfs/blob/941b616/docs/api/batch.md";

        private static readonly IImmutableSet<string> ValidHashAlgorithms =
            ImmutableSortedSet.Create("sha256");

        private static readonly Action<ILogger, string, long, Exception?> LogReceivedOidAndSize =
            LoggerMessage.Define<string, long>(
                LogLevel.Debug,
                new EventId(1, "ReceivedOidAndSize"),
                "Received oid={Oid} and size={Size}.");

        private readonly ILogger<ObjectsController> logger;
        private readonly ILfsObjectManager lfsObjectManager;

        public ObjectsController(ILogger<ObjectsController> logger, ILfsObjectManager lfsObjectManager)
        {
            this.logger = logger;
            this.lfsObjectManager = lfsObjectManager;
        }

        [HttpPost("batch")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status410Gone)]
        [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
        [ProducesResponseType(StatusCodes.Status413RequestEntityTooLarge)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status501NotImplemented)]
        [ProducesResponseType(StatusCodes.Status507InsufficientStorage)]
        [ProducesResponseType(/* bandwidth limit exceeded */509)]
        public async Task<ActionResult<BatchResponse>> BatchAsync(
            [FromRoute] string org,
            [FromRoute] string repo,
            [FromBody] BatchRequest request,
            CancellationToken cancellationToken)
        {
            using var activity = new Activity("GitLfsService.Api.Controllers.ObjectsController.BatchAsync")
                .Start();

            // TODO(hcoona): verify authentication before processing, maybe via a filter. 401.
            // TODO(hcoona): verify authorization before processing, maybe via a filter. 403.
            // TODO(hcoona): verify org/repo existance. 404.
            // TODO(hcoona): verify ratelimit/bandwidth limit. 429/509.
            if (!ValidHashAlgorithms.Contains(request.HashAlgorithm))
            {
                return this.Conflict(
                    new ErrorResponse
                    {
                        Message = $"Unsupported hash algorithm: {request.HashAlgorithm}",
                        RequestId = activity.Id ?? this.HttpContext.TraceIdentifier,
                        DocumentationUrl = GitLfsBatchApiSpecificationUrl,
                    });
            }

            try
            {
                if (request.Operator == "download")
                {
                    return await this.lfsObjectManager.PrepareDownloadObjects(
                        new ILfsObjectManager.BatchRequest(
                            org,
                            repo,
                            request.Transfers,
                            (from obj in request.Objects
                             select new ILfsObjectManager.LfsObjectInfo(
                                obj.ObjectId,
                                obj.Size)).ToImmutableList(),
                            request.HashAlgorithm),
                        cancellationToken);
                }
                else if (request.Operator == "upload")
                {
                    return await this.lfsObjectManager.PrepareUploadObjects(
                        new ILfsObjectManager.BatchRequest(
                            org,
                            repo,
                            request.Transfers,
                            (from obj in request.Objects
                             select new ILfsObjectManager.LfsObjectInfo(
                                obj.ObjectId,
                                obj.Size)).ToImmutableList(),
                            request.HashAlgorithm),
                        cancellationToken);
                }
                else
                {
                    return this.UnprocessableEntity(
                        new ErrorResponse
                        {
                            Message = $"Invalid operator '{request.Operator}',"
                                + " only 'download' and 'upload' are supported.",
                            RequestId = activity.Id ?? this.HttpContext.TraceIdentifier,
                            DocumentationUrl = GitLfsBatchApiSpecificationUrl,
                        });
                }
            }
            catch (NotImplementedException ex)
            {
                return this.StatusCode(
                    StatusCodes.Status501NotImplemented,
                    new ErrorResponse
                    {
                        Message = ex.Message,
                        RequestId = activity.Id ?? this.HttpContext.TraceIdentifier,
                        DocumentationUrl = GitLfsBatchApiSpecificationUrl,
                    });
            }
        }

        [HttpPut("basic_upload/{oid}")]
        [Consumes("application/octet-stream")]
        public async Task<ActionResult> BasicTransferUpload(
            [FromRoute] string org,
            [FromRoute] string repo,
            [FromRoute] string oid,
            CancellationToken cancellationToken)
        {
            using var activity = new Activity("GitLfsService.Api.Controllers.ObjectsController.BasicTransferUpload")
                .Start();

            using (var memoryStream = new MemoryStream())
            {
                await this.HttpContext.Request.BodyReader.CopyToAsync(memoryStream, cancellationToken);

                LogReceivedOidAndSize(this.logger, oid, memoryStream.Length, null);
            }

            return this.Ok();
        }
    }
}
