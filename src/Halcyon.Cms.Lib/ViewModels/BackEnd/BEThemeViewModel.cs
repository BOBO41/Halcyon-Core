﻿// Licensed to the Halcyon Core Foundation under one or more agreements.
// The Halcyon Core Foundation licenses this file to you under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Halcyon.Cms.Lib.Models.Cms;
using Halcyon.Cms.Lib.Repositories;
using Halcyon.Cms.Lib.ViewModels.Info;
using Halcyon.Common.Helper;
using Halcyon.Domain.Core.ViewModels;
using Halcyon.Domain.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Halcyon.Cms.Lib.ViewModels.BackEnd
{
    public class BEThemeViewModel
       : ViewModelBase<SiocCmsContext, SiocTheme, BEThemeViewModel>
    {
        public const int templatePageSize = 10;

        #region Properties

        #region Models

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }

        #endregion Models

        #region Views

        [JsonProperty("isActived")]
        public bool IsActived { get; set; }

        [JsonProperty("asset")]
        public IFormFile Asset { get; set; }// = new FileViewModel();

        [JsonProperty("assetFolder")]
        public string AssetFolder
        {
            get
            {
                return CommonHelper.GetFullPath(new string[] {
                    SWCmsConstants.Parameters.FileFolder,
                    SWCmsConstants.Parameters.TemplatesAssetFolder,
                    Name });
            }
        }

        [JsonProperty("templateFolder")]
        public string TemplateFolder
        {
            get
            {
                return CommonHelper.GetFullPath(new string[] { SWCmsConstants.Parameters.TemplatesFolder, Name });
            }
        }

        public List<BETemplateViewModel> Templates { get; set; }

        #endregion Views

        #endregion Properties

        #region Contructors

        public BEThemeViewModel()
            : base()
        {
        }

        public BEThemeViewModel(SiocTheme model, SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
            : base(model, _context, _transaction)
        {
        }

        #endregion Contructors

        #region Overrides

        public override SiocTheme ParseModel(SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            if (Id == 0)
            {
                CreatedDateTime = DateTime.UtcNow;
            }
            return base.ParseModel();
        }

        public override void ExpandView(SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            Templates = BETemplateViewModel.Repository.GetModelListBy(t => t.TemplateId == Id,
                _context: _context, _transaction: _transaction).Data;
        }

        #region Sync

        public override RepositoryResponse<BEThemeViewModel> SaveModel(bool isSaveSubModels = false, SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            var result = base.SaveModel(isSaveSubModels, _context, _transaction);
            if (result.IsSucceed)
            {
                if (IsActived)
                {
                    ConfigurationViewModel config = (ConfigurationViewModel.Repository.GetSingleModel(
                        c => c.Keyword == SWCmsConstants.ConfigurationKeyword.Theme, _context, _transaction)).Data;
                    if (config == null)
                    {
                        config = new ConfigurationViewModel()
                        {
                            Keyword = SWCmsConstants.ConfigurationKeyword.Theme,
                            Specificulture = !string.IsNullOrEmpty(Specificulture) ? Specificulture : SWCmsConstants.Default.Specificulture,
                            Category = SWCmsConstants.ConfigurationType.User,
                            DataType = SWCmsConstants.DataType.String,
                            Description = "Cms Theme",

                            Value = Name
                        };
                    }
                    else
                    {
                        config.Value = Name;
                    }

                    var saveConfigResult = config.SaveModel(false, _context, _transaction);
                    if (!saveConfigResult.IsSucceed)
                    {
                        Errors.AddRange(saveConfigResult.Errors);
                    }
                    result.IsSucceed = result.IsSucceed && saveConfigResult.IsSucceed;

                    ConfigurationViewModel configId = (ConfigurationViewModel.Repository.GetSingleModel(
                          c => c.Keyword == SWCmsConstants.ConfigurationKeyword.ThemeId, _context, _transaction)).Data;
                    if (configId == null)
                    {
                        configId = new ConfigurationViewModel()
                        {
                            Keyword = SWCmsConstants.ConfigurationKeyword.ThemeId,
                            Specificulture = Specificulture,
                            Category = SWCmsConstants.ConfigurationType.User,
                            DataType = SWCmsConstants.DataType.String,
                            Description = "Cms Theme Id",
                            Value = Model.Id.ToString()
                        };
                    }
                    else
                    {
                        configId.Value = Model.Id.ToString();
                    }
                    var saveResult = configId.SaveModel(false, _context, _transaction);
                    if (!saveResult.IsSucceed)
                    {
                        Errors.AddRange(saveResult.Errors);
                    }
                    result.IsSucceed = result.IsSucceed && saveResult.IsSucceed;
                }

                if (Asset != null && Asset.Length > 0 && Id == 0)
                {
                    var files = FileRepository.Instance.GetWebFiles(AssetFolder);
                    string strStyles = string.Empty;
                    foreach (var css in files.Where(f => f.Extension == ".css"))
                    {
                        strStyles += string.Format(@"   <link href='{0}/{1}{2}' rel='stylesheet'/>
", css.FileFolder, css.Filename, css.Extension);
                    }
                    string strScripts = string.Empty;
                    foreach (var js in files.Where(f => f.Extension == ".js"))
                    {
                        strScripts += string.Format(@"  <script src='{0}/{1}{2}'></script>
", js.FileFolder, js.Filename, js.Extension);
                    }
                    var layout = BETemplateViewModel.Repository.GetSingleModel(
                        t => t.FileName == "_Layout" && t.TemplateId == Model.Id
                        , _context, _transaction);
                    layout.Data.Content = layout.Data.Content.Replace("<!--[STYLES]-->"
                        , string.Format(@"{0}"
                        , strStyles));
                    layout.Data.Content = layout.Data.Content.Replace("<!--[SCRIPTS]-->"
                        , string.Format(@"{0}"
                        , strScripts));

                    layout.Data.SaveModel(true, _context, _transaction);
                }
            }
            return result;
        }

        public override RepositoryResponse<bool> SaveSubModels(SiocTheme parent, SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            RepositoryResponse<bool> result = new RepositoryResponse<bool>() { IsSucceed = true };

            if (Asset != null && Asset.Length > 0)
            {
                string filename = FileRepository.Instance.SaveWebFile(Asset, AssetFolder);
                if (!string.IsNullOrEmpty(filename))
                {
                    FileRepository.Instance.UnZipFile(filename, AssetFolder);
                }
            }
            if (Id == 0)
            {
                string defaultFolder = CommonHelper.GetFullPath(new string[] { SWCmsConstants.Parameters.TemplatesFolder, Name == "Default" ? "Default" : SWCmsConstants.Default.DefaultTemplateFolder });
                bool copyResult = FileRepository.Instance.CopyDirectory(defaultFolder, TemplateFolder);
                var files = copyResult ? FileRepository.Instance.GetFilesWithContent(TemplateFolder) : new System.Collections.Generic.List<FileViewModel>();
                foreach (var file in files)
                {
                    BETemplateViewModel template = new BETemplateViewModel(
                        new SiocTemplate()
                        {
                            FileFolder = file.FileFolder,
                            FileName = file.Filename,
                            Content = file.Content,
                            Extension = file.Extension,
                            CreatedDateTime = DateTime.UtcNow,
                            LastModified = DateTime.UtcNow,
                            TemplateId = Model.Id,
                            TemplateName = Model.Name,
                            FolderType = file.FolderName,
                            ModifiedBy = CreatedBy
                        });
                    var saveResult = template.SaveModel(true, _context, _transaction);
                    result.IsSucceed = result.IsSucceed && saveResult.IsSucceed;
                    if (!saveResult.IsSucceed)
                    {
                        result.Exception = saveResult.Exception;
                        result.Errors.AddRange(saveResult.Errors);
                        break;
                    }
                }
            }
            return result;
        }

        public override RepositoryResponse<bool> RemoveRelatedModels(BEThemeViewModel view, SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            RepositoryResponse<bool> result = new RepositoryResponse<bool>() { IsSucceed = true };
            result = InfoTemplateViewModel.Repository.RemoveListModel(t => t.TemplateId == Id);
            if (result.IsSucceed)
            {
                FileRepository.Instance.DeleteWebFolder(AssetFolder);
                FileRepository.Instance.DeleteFolder(TemplateFolder);
            }
            return result;
        }

        #endregion Sync

        #region Async

        public override async Task<RepositoryResponse<BEThemeViewModel>> SaveModelAsync(bool isSaveSubModels = false, SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            var result = await base.SaveModelAsync(isSaveSubModels, _context, _transaction);
            if (result.IsSucceed)
            {
                if (IsActived)
                {
                    ConfigurationViewModel config = (await ConfigurationViewModel.Repository.GetSingleModelAsync(
                        c => c.Keyword == SWCmsConstants.ConfigurationKeyword.Theme, _context, _transaction)).Data;
                    if (config == null)
                    {
                        config = new ConfigurationViewModel()
                        {
                            Keyword = SWCmsConstants.ConfigurationKeyword.Theme,
                            Specificulture = Specificulture,
                            Category = SWCmsConstants.ConfigurationType.User,
                            DataType = SWCmsConstants.DataType.String,
                            Description = "Cms Theme",
                            Value = Name
                        };
                    }
                    else
                    {
                        config.Value = Name;
                    }

                    var saveConfigResult = await config.SaveModelAsync(false, _context, _transaction);
                    if (!saveConfigResult.IsSucceed)
                    {
                        Errors.AddRange(saveConfigResult.Errors);
                    }
                    result.IsSucceed = result.IsSucceed && saveConfigResult.IsSucceed;

                    ConfigurationViewModel configId = (await ConfigurationViewModel.Repository.GetSingleModelAsync(
                          c => c.Keyword == SWCmsConstants.ConfigurationKeyword.ThemeId, _context, _transaction)).Data;
                    if (configId == null)
                    {
                        configId = new ConfigurationViewModel()
                        {
                            Keyword = SWCmsConstants.ConfigurationKeyword.ThemeId,
                            Specificulture = Specificulture,
                            Category = SWCmsConstants.ConfigurationType.User,
                            DataType = SWCmsConstants.DataType.String,
                            Description = "Cms Theme Id",
                            Value = Model.Id.ToString()
                        };
                    }
                    else
                    {
                        configId.Value = Model.Id.ToString();
                    }
                    var saveResult = await configId.SaveModelAsync(false, _context, _transaction);
                    if (!saveResult.IsSucceed)
                    {
                        Errors.AddRange(saveResult.Errors);
                    }
                    result.IsSucceed = result.IsSucceed && saveResult.IsSucceed;
                }

                if (Asset != null && Asset.Length > 0 && Id == 0)
                {
                    var files = FileRepository.Instance.GetWebFiles(AssetFolder);
                    string strStyles = string.Empty;
                    foreach (var css in files.Where(f => f.Extension == ".css"))
                    {
                        strStyles += string.Format(@"   <link href='{0}/{1}{2}' rel='stylesheet'/>
", css.FileFolder, css.Filename, css.Extension);
                    }
                    string strScripts = string.Empty;
                    foreach (var js in files.Where(f => f.Extension == ".js"))
                    {
                        strScripts += string.Format(@"  <script src='{0}/{1}{2}'></script>
", js.FileFolder, js.Filename, js.Extension);
                    }
                    var layout = BETemplateViewModel.Repository.GetSingleModel(
                        t => t.FileName == "_Layout" && t.TemplateId == Model.Id
                        , _context, _transaction);
                    layout.Data.Content = layout.Data.Content.Replace("<!--[STYLES]-->"
                        , string.Format(@"{0}"
                        , strStyles));
                    layout.Data.Content = layout.Data.Content.Replace("<!--[SCRIPTS]-->"
                        , string.Format(@"{0}"
                        , strScripts));

                    await layout.Data.SaveModelAsync(true, _context, _transaction);
                }
            }
            return result;
        }

        public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(SiocTheme parent, SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            RepositoryResponse<bool> result = new RepositoryResponse<bool>() { IsSucceed = true };

            if (Asset != null && Asset.Length > 0)
            {
                string filename = FileRepository.Instance.SaveWebFile(Asset, AssetFolder);
                if (!string.IsNullOrEmpty(filename))
                {
                    FileRepository.Instance.UnZipFile(filename, AssetFolder);
                }
            }
            if (Id == 0)
            {
                string defaultFolder = CommonHelper.GetFullPath(new string[] { SWCmsConstants.Parameters.TemplatesFolder, Name == "Default" ? "Default" : SWCmsConstants.Default.DefaultTemplateFolder });
                bool copyResult = FileRepository.Instance.CopyDirectory(defaultFolder, TemplateFolder);
                var files = copyResult ? FileRepository.Instance.GetFilesWithContent(TemplateFolder) : new System.Collections.Generic.List<FileViewModel>();
                //TODO: Create default asset
                //FileRepository.Instance.CopyDirectory(TemplateFolder, TemplateFolder);
                foreach (var file in files)
                {
                    BETemplateViewModel template = new BETemplateViewModel(
                        new SiocTemplate()
                        {
                            FileFolder = file.FileFolder,
                            FileName = file.Filename,
                            Content = file.Content,
                            Extension = file.Extension,
                            CreatedDateTime = DateTime.UtcNow,
                            LastModified = DateTime.UtcNow,
                            TemplateId = Model.Id,
                            TemplateName = Model.Name,
                            FolderType = file.FolderName,
                            ModifiedBy = CreatedBy
                        });
                    var saveResult = await template.SaveModelAsync(true, _context, _transaction);
                    result.IsSucceed = result.IsSucceed && saveResult.IsSucceed;
                    if (!saveResult.IsSucceed)
                    {
                        result.Exception = saveResult.Exception;
                        result.Errors.AddRange(saveResult.Errors);
                        break;
                    }
                }
            }
            return result;
        }

        public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(BEThemeViewModel view, SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            RepositoryResponse<bool> result = new RepositoryResponse<bool>() { IsSucceed = true };
            result = await InfoTemplateViewModel.Repository.RemoveListModelAsync(t => t.TemplateId == Id);
            if (result.IsSucceed)
            {
                FileRepository.Instance.DeleteWebFolder(AssetFolder);
                FileRepository.Instance.DeleteFolder(TemplateFolder);
            }
            return result;
        }

        #endregion Async

        #endregion Overrides
    }
}
