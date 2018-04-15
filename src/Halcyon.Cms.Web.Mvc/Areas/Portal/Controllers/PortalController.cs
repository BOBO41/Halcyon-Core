﻿// Licensed to the Halcyon Core Foundation under one or more agreements.
// The Halcyon Core Foundation licenses this file to you under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Halcyon.Cms.Lib;
using Halcyon.Cms.Lib.Repositories;
using Halcyon.Cms.Lib.Services;
using Halcyon.Cms.Lib.ViewModels;
using Halcyon.Cms.Lib.ViewModels.Info;
using Halcyon.Cms.Mvc.Controllers;
using System.Threading.Tasks;

namespace Halcyon.Cms.Mvc.Areas.Portal.Controllers
{
    [Area("Portal")]
    [Route("{culture}/Portal")]
    public class PortalController : BaseController<PortalController>
    {
        public PortalController(IHostingEnvironment env, IConfigurationRoot configuration)
            : base(env, configuration)
        {
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return RedirectToAction("", "Dashboard", new { culture = CurrentLanguage });
            //return View();
        }

        [HttpGet]
        [Route("init")]
        public IActionResult Init()
        {
            var model = new InitCmsViewModel() {
                LocalDbName = "aspnet-halcyon-v1.Cms.Db"
            };
            return View(model);
        }

        [HttpPost]
        [Route("init")]
        public IActionResult Init(InitCmsViewModel model)
        {
            if (ModelState.IsValid)
            {
                string cnnString = string.Empty;
                if (!model.IsUseLocal)
                {
                    cnnString = string.Format("Server={0};Database={1};UID={2};Pwd={3};MultipleActiveResultSets=true;"
                       , model.DataBaseServer, model.DataBaseName, model.DataBaseUser, model.DataBasePassword);
                }
                else
                {
                    cnnString = $"Server=(localdb)\\mssqllocaldb;Database={model.LocalDbName};Trusted_Connection=True;MultipleActiveResultSets=true";
                }

                GlobalConfigurationService.Instance.ConnectionString = cnnString;
                GlobalConfigurationService.Instance.InitSWCms();
                if (GlobalConfigurationService.Instance.IsInit)
                {
                    
                    var settings = FileRepository.Instance.GetFile("appsettings", ".json", string.Empty);
                    if (settings != null)
                    {
                        JObject jsonSettings = JObject.Parse(settings.Content);
                        jsonSettings["ConnectionStrings"][SWCmsConstants.CONST_DEFAULT_CONNECTION] = cnnString;
                        settings.Content = jsonSettings.ToString();
                        FileRepository.Instance.SaveFile(settings);
                    }
                    return RedirectToAction("Register", "Auth", new { culture = SWCmsConstants.Default.Specificulture });
                }
            }
            return View(model);
        }

        /// <summary>
        /// Searches the specified keyword.
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        /// <param name="searchType">Type of the search Ex: All / Article / Module / Page .</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        [Route("Search")]
        public async Task<IActionResult> Search(string keyword, SWCmsConstants.SearchType searchType)
        {
            switch (searchType)
            {
                case SWCmsConstants.SearchType.All:
                    ViewData["Articles"] = (await InfoArticleViewModel.Repository.GetModelListByAsync(
                        c => c.Specificulture == CurrentLanguage && (c.Title.Contains(keyword) || c.Excerpt.Contains(keyword) || c.Content.Contains(keyword))).ConfigureAwait(false)
                        ).Data;
                    ViewData["Pages"] = (InfoCategoryViewModel.Repository.GetModelListBy(
                        c => c.Specificulture == CurrentLanguage
                        && (c.Title.Contains(keyword) || c.Excerpt.Contains(keyword)))
                        ).Data;
                    ViewData["Modules"] = (InfoModuleViewModel.Repository.GetModelListBy(
                        c => c.Specificulture == CurrentLanguage && (c.Title.Contains(keyword)
                        || c.Description.Contains(keyword)))
                        ).Data;
                    break;

                case SWCmsConstants.SearchType.Article:
                    ViewData["Articles"] = (await InfoArticleViewModel.Repository.GetModelListByAsync(
                        c => c.Specificulture == CurrentLanguage && (c.Title.Contains(keyword) || c.Excerpt.Contains(keyword) || c.Content.Contains(keyword))).ConfigureAwait(false)
                        ).Data;
                    break;

                case SWCmsConstants.SearchType.Module:
                    ViewData["Modules"] = (InfoModuleViewModel.Repository.GetModelListBy(
                        c => c.Specificulture == CurrentLanguage && (c.Title.Contains(keyword) || c.Description.Contains(keyword)))
                        ).Data;
                    break;

                case SWCmsConstants.SearchType.Page:
                    ViewData["Pages"] = (InfoCategoryViewModel.Repository.GetModelListBy(
                        c => c.Specificulture == CurrentLanguage
                        && (c.Title.Contains(keyword) || c.Excerpt.Contains(keyword)))
                        ).Data;
                    break;

                default:
                    break;
            }
            ViewBag.searchType = searchType;
            return View();
        }
    }
}
