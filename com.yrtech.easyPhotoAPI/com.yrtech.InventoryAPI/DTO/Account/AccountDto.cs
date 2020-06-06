﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.InventoryAPI.DTO
{
    [Serializable]
    public class AccountDto
    {
        public int TenantId { get; set; }
        public int BrandId { get; set; }
        public string TenantName { get; set;  }
        public string TenantCode { get; set; }
        public string BrandName { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public int UserId { get; set;  }
        public string Password { get; set; }
        public bool UseChk { get; set; }
        public string TelNO { get; set; }
        public string Email { get; set; }
        public string HeadPicUrl { get; set; }
    }
}