﻿// Licensed to the Halcyon Core Foundation under one or more agreements.
// The Halcyon Core Foundation licenses this file to you under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

namespace Halcyon.Cms.Lib.Models.Account
{
    public partial class Clients
    {
        public string Id { get; set; }
        public bool Active { get; set; }
        public string AllowedOrigin { get; set; }
        public int ApplicationType { get; set; }
        public string Name { get; set; }
        public int RefreshTokenLifeTime { get; set; }
        public string Secret { get; set; }
    }
}
