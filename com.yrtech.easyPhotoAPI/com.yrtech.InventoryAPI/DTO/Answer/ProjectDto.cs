using com.yrtech.InventoryDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.InventoryAPI.DTO
{
    [Serializable]
    public class ProjectDto
    {
        public int ProjectId { get; set; }
        public Nullable<int> TenantId { get; set; }
        public Nullable<int> BrandId { get; set; }
        public string BrandName { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> ExpireDateTime { get; set; }
        public string ShopCode { get; set; }
        public string Quarter { get; set; }
        public string Year { get; set; }
        public Nullable<int> OrderNO { get; set; }
        public Nullable<bool> ExportRecheck { get; set; }
        public string InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public string ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }
    }
}