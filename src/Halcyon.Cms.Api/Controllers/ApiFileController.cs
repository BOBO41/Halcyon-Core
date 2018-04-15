﻿// Licensed to the Halcyon Core Foundation under one or more agreements.
// The Halcyon Core Foundation licenses this file to you under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Mvc;
using Halcyon.Api.Controllers;
using Halcyon.Cms.Lib.Repositories;
using Halcyon.Cms.Lib.ViewModels;

namespace Swastka.IO.Cms.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/file")]
    public class ApiFileController : BaseApiController
    {
        // POST api/values
        /// <summary>
        /// Uploads the image.
        /// </summary>
        /// <param name="image">The img information.</param>
        /// <param name="template"></param> Ex: { "base64": "", "fileFolder":"" }
        /// <returns></returns>
        [Route("uploadFile")]
        [HttpPost]
        public IActionResult Edit(FileViewModel template)
        {
            if (ModelState.IsValid)
            {
                var result = FileRepository.Instance.SaveWebFile(template);
                return GetSuccessResult(result);
            }
            return GetErrorResult("failed", "invalid");
        }
    }
}
