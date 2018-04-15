﻿// Licensed to the Halcyon Core Foundation under one or more agreements.
// The Halcyon Core Foundation licenses this file to you under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Halcyon.Cms.Lib.Models.Cms;
using Halcyon.Cms.Lib.ViewModels.Info;
using Halcyon.Domain.Data.ViewModels;
using System;

namespace Halcyon.Cms.Lib.ViewModels.Navigation
{
    public class NavRelatedProductViewModel
        : ViewModelBase<SiocCmsContext, SiocRelatedProduct, NavRelatedProductViewModel>
    {
        #region Properties

        #region Models

        [JsonProperty("sourceProductId")]
        public string SourceProductId { get; set; }

        [JsonProperty("relatedProductId")]
        public string RelatedProductId { get; set; }

        [JsonProperty("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        #endregion Models

        #region Views

        [JsonProperty("isActived")]
        public bool IsActived { get; set; }

        [JsonProperty("relatedProduct")]
        public InfoProductViewModel RelatedProduct { get; set; }

        #endregion Views

        #endregion Properties

        #region Contructors

        public NavRelatedProductViewModel() : base()
        {
        }

        public NavRelatedProductViewModel(SiocRelatedProduct model, SiocCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
        {
        }

        #endregion Contructors

        #region Overrides

        public override void ExpandView(SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            var getProduct = InfoProductViewModel.Repository.GetSingleModel(
                m => m.Id == RelatedProductId && m.Specificulture == Specificulture
                , _context: _context, _transaction: _transaction);
            if (getProduct.IsSucceed)
            {
                this.RelatedProduct = getProduct.Data;
            }
        }

        public override SiocRelatedProduct ParseModel(SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            if (CreatedDateTime == default(DateTime))
            {
                CreatedDateTime = DateTime.UtcNow;
            }
            return base.ParseModel();
        }

        #endregion Overrides
    }
}
