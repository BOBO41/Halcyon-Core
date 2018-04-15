﻿// Licensed to the Halcyon Core Foundation under one or more agreements.
// The Halcyon Core Foundation licenses this file to you under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

using System;

namespace Halcyon.Cms.Lib.Models.Account
{
    public partial class RefreshTokens
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string Email { get; set; }
        public DateTime ExpiresUtc { get; set; }
        public DateTime IssuedUtc { get; set; }
    }
}
