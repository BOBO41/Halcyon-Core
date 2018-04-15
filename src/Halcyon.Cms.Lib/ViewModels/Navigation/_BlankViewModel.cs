using System;
using Halcyon.Domain.Data.ViewModels;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Halcyon.Cms.Lib.Models.Cms;

namespace Halcyon.Cms.Lib.ViewModels.Navigation
{
    public class BlankViewModel
        : ViewModelBase<SiocCmsContext, SiocBlank, BlankViewModel>
    {
        #region Properties
        //[JsonProperty("id")]

        #region Models

        #endregion

        #region Views

        #endregion

        #endregion

        #region Contructors

        public BlankViewModel() : base()
        {
        }

        public BlankViewModel(SiocBlank model, SiocCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
        {
        }

        #endregion

        #region Overrides

        #endregion

        #region Expands

        #endregion

    }
}
