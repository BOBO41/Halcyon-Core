﻿// Licensed to the Halcyon Core Foundation under one or more agreements.
// The Halcyon Core Foundation licenses this file to you under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Halcyon.Cms.Lib.Models.Cms
{
    public partial class SiocTheme
    {
        public SiocTheme()
        {
            SiocFile = new HashSet<SiocFile>();
            SiocTemplate = new HashSet<SiocTemplate>();
        }

        public int Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public int Status { get; set; }

        public ICollection<SiocFile> SiocFile { get; set; }
        public ICollection<SiocTemplate> SiocTemplate { get; set; }
    }
}
