﻿// Licensed to the Halcyon Core Foundation under one or more agreements.
// The Halcyon Core Foundation licenses this file to you under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Halcyon.Cms.Lib.Models.Cms
{
    public partial class SiocModuleAttributeSet
    {
        public SiocModuleAttributeSet()
        {
            SiocModuleAttributeValue = new HashSet<SiocModuleAttributeValue>();
        }

        public Guid Id { get; set; }
        public int ModuleId { get; set; }
        public string Specificulture { get; set; }
        public string ArticleId { get; set; }
        public int? CategoryId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string Fields { get; set; }
        public int Priority { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public string Value { get; set; }
        public int Status { get; set; }

        public SiocArticleModule SiocArticleModule { get; set; }
        public SiocCategoryModule SiocCategoryModule { get; set; }
        public SiocModule SiocModule { get; set; }
        public ICollection<SiocModuleAttributeValue> SiocModuleAttributeValue { get; set; }
    }
}
