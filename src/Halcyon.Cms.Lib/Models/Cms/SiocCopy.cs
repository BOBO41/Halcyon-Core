﻿// Licensed to the Halcyon Core Foundation under one or more agreements.
// The Halcyon Core Foundation licenses this file to you under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

namespace Halcyon.Cms.Lib.Models.Cms
{
    public partial class SiocCopy
    {
        public string Culture { get; set; }
        public string Keyword { get; set; }
        public string Note { get; set; }
        public string Value { get; set; }
        public int Priority { get; set; }
        public int Status { get; set; }
    }
}
