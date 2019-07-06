using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.InventoryAPI.DTO
{
    public class ShopDto
    {
        public int TenantId { get; set; }
        public string TenantCode { get; set; }
        public string TenantName { get; set; }
        public int BrandId { get; set; }
        public string BrandCode { get; set;  }
        public string BrandName { get; set; }
        public int ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string AreaCode { get; set; }
        public int ShopId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string Password { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string UserId { get; set; }
        public DateTime ExpirTime { get; set; }
        public string InUserId { get; set; }
        public DateTime InDateTime { get; set; }
        public string ModifyUserId { get; set; }
        public DateTime ModifyDateTime { get; set; }
    }
}