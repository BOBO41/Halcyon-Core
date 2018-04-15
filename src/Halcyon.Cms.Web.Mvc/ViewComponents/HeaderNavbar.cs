﻿// Licensed to the Halcyon Core Foundation under one or more agreements.
// The Halcyon Core Foundation licenses this file to you under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Mvc;
using Halcyon.Cms.Lib;
using Halcyon.Cms.Lib.ViewModels.Info;
using System.Linq;

namespace Halcyon.Cms.Mvc.ViewComponents
{
    public class HeaderNavbar : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            string culture = ViewBag.culture;
            var topCates = InfoCategoryViewModel.Repository.GetModelListBy
                (c => c.Specificulture == culture && c.SiocCategoryPosition.Count(p => p.PositionId == (int)SWCmsConstants.CatePosition.Top) > 0

                );
            var data = topCates.Data ?? new System.Collections.Generic.List<InfoCategoryViewModel>();
            return View(data);
        }
    }
}