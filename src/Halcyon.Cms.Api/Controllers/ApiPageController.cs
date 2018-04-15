﻿// Licensed to the Halcyon Core Foundation under one or more agreements.
// The Halcyon Core Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.OData.Query;
using Newtonsoft.Json.Linq;
using Halcyon.Api.Controllers;
using Halcyon.Cms.Lib;
using Halcyon.Cms.Lib.Models.Cms;
using Halcyon.Cms.Lib.ViewModels.BackEnd;
using Halcyon.Cms.Lib.ViewModels.FrontEnd;
using Halcyon.Cms.Lib.ViewModels.Info;
using Halcyon.Domain.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Halcyon.Common.Utility.Enums;

namespace Halcyon.IO.Cms.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/{culture}/page")]
    public class ApiCategoryController :
        BaseApiController<SiocCmsContext, SiocCategory>
    {
        #region Get

        // GET api/page/details/spa/1
        [HttpGet]
        [Route("details/{viewType}/{id}")]
        public async Task<JObject> DetailsByType(string viewType, int id)
        {
            switch (viewType)
            {
                case "fe":
                    var spaResult = await FECategoryViewModel.Repository.GetSingleModelAsync(model => model.Id == id && model.Specificulture == _lang).ConfigureAwait(false);
                    return JObject.FromObject(spaResult);

                case "be":
                    var beResult = await BECategoryViewModel.Repository.GetSingleModelAsync(model => model.Id == id && model.Specificulture == _lang).ConfigureAwait(false);
                    return JObject.FromObject(beResult);

                default:
                    var feResult = await FECategoryViewModel.Repository.GetSingleModelAsync(model => model.Id == id && model.Specificulture == _lang).ConfigureAwait(false);
                    return JObject.FromObject(feResult);
            }
        }

        // GET api/page/id
        [HttpGet]
        [Route("byArticle/{id}")]
        [Route("byArticle/{id}/{articleId}")]
        public Task<RepositoryResponse<FECategoryViewModel>> GetByArticle(int id, string articleId = null)
        {
            return FECategoryViewModel.Repository.GetSingleModelAsync(
                model => model.Id == id && model.Specificulture == _lang);
        }

        // GET api/Category
        [HttpGet]
        [Route("list")]
        [Route("list/{PageSize:int?}/{PageIndex:int?}")]
        [Route("list/{orderBy}/{direction}")]
        [Route("list/{PageSize:int?}/{PageIndex:int?}/{orderBy}/{direction}")]
        public async Task<RepositoryResponse<PaginationModel<InfoCategoryViewModel>>> Get(
            int? PageSize = 15, int? PageIndex = 0, string orderBy = "Id"
            , OrderByDirection direction = OrderByDirection.Ascending)
        {
            var data = await InfoCategoryViewModel.Repository.GetModelListByAsync(m => m.Specificulture == _lang, orderBy, direction, PageSize, PageIndex).ConfigureAwait(false); //base.Get(orderBy, direction, PageSize, PageIndex);
            string domain = string.Format("{0}://{1}", Request.Scheme, Request.Host);
            //data.Data.Items.ForEach(d => d.DetailsUrl = string.Format("{0}{1}", domain, this.Url.Action("Details", "Category", new { id = d.Id })));
            //data.Data.Items.ForEach(d => d.EditUrl = string.Format("{0}{1}", domain, this.Url.Action("Edit", "Category", new { id = d.Id })));
            return data;
        }

        // GET api/Category
        [HttpGet]
        [Route("search/{keyword}")]
        [Route("search/{PageSize:int?}/{PageIndex:int?}/{keyword}")]
        [Route("search/{PageSize:int?}/{PageIndex:int?}/{keyword}/{description}")]
        [Route("search/{PageSize:int?}/{PageIndex:int?}/{orderBy}/{direction}/{keyword}")]
        [Route("search/{PageSize:int?}/{PageIndex:int?}/{orderBy}/{direction}/{keyword}/{description}")]
        public Task<RepositoryResponse<PaginationModel<InfoCategoryViewModel>>> Search(
            string keyword = null,
            string description = null,
            int? PageSize = null, int? PageIndex = null, string orderBy = "Id"
            , OrderByDirection direction = OrderByDirection.Ascending)
        {
            Expression<Func<SiocCategory, bool>> predicate = model =>
            model.Specificulture == _lang
            && (string.IsNullOrWhiteSpace(keyword) || (model.Title.Contains(keyword)))
            && (string.IsNullOrWhiteSpace(description) || (model.Excerpt.Contains(description)));
            return InfoCategoryViewModel
                .Repository
                .GetModelListByAsync(predicate, orderBy, direction, PageSize, PageIndex); // base.Search(predicate, orderBy, direction, PageSize, PageIndex, keyword);
        }

        #endregion Get

        #region Post

        // POST api/page
        [HttpPost, HttpOptions]
        [Route("save")]
        public async Task<RepositoryResponse<BECategoryViewModel>> Post([FromBody]BECategoryViewModel model)
        {
            if (model != null)
            {
                var result = await model.SaveModelAsync(true).ConfigureAwait(false);
                if (result.IsSucceed)
                {
                    result.Data.Domain = this._domain;
                }
                return result;
            }
            return new RepositoryResponse<BECategoryViewModel>();
        }

        // POST api/page
        [HttpPost, HttpOptions]
        [Route("save/{id}")]
        public async Task<RepositoryResponse<bool>> SaveFields(int id, [FromBody]List<EntityField> fields)
        {
            if (fields != null)
            {
                foreach (var property in fields)
                {
                    var result = await InfoCategoryViewModel.Repository.UpdateFieldsAsync(c => c.Id == id && c.Specificulture == _lang, fields).ConfigureAwait(false);

                    return result;
                }
            }
            return new RepositoryResponse<bool>();
        }

        // GET api/page
        [HttpPost, HttpOptions]
        [Route("list")]
        public async Task<RepositoryResponse<PaginationModel<InfoCategoryViewModel>>> GetList(RequestPaging request)
        {
            string domain = string.Format("{0}://{1}", Request.Scheme, Request.Host);

            Expression<Func<SiocCategory, bool>> predicate = model =>
                model.Specificulture == _lang
                && (string.IsNullOrWhiteSpace(request.Keyword)
                    || (model.Title.Contains(request.Keyword)
                    || model.Excerpt.Contains(request.Keyword)))
                && (!request.FromDate.HasValue
                    || (model.CreatedDateTime >= request.FromDate.Value.ToUniversalTime())
                )
                && (!request.ToDate.HasValue
                    || (model.CreatedDateTime <= request.ToDate.Value.ToUniversalTime())
                )
                    ;

            var data = await InfoCategoryViewModel.Repository.GetModelListByAsync(predicate, request.OrderBy, request.Direction, request.PageSize, request.PageIndex).ConfigureAwait(false);
            if (data.IsSucceed)
            {
                data.Data.Items.ForEach(a =>
                {
                    a.DetailsUrl = SWCmsHelper.GetRouterUrl(
                        "Page", new { a.SeoName }, Request, Url);
                    a.Domain = domain;
                }
                );
            }
            return data;
        }

        #endregion Post
    }
}
