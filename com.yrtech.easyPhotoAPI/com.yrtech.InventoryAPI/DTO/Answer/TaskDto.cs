using com.yrtech.InventoryDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.InventoryAPI.DTO
{
    [Serializable]
    public class TaskDto
    {
        public int TaskId { get; set; }
        public int ProjectId { get; set; }
        public Nullable<int> TenantId { get; set; }
        public Nullable<int> BrandId { get; set; }
        public string BrandName { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string TaskCode { get; set; }
        public string TaskName { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public string InventoryDayCode { get; set; } // 盘点码

        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> ExpireDateTime { get; set; }
        public string ShopCode { get; set; }
        public int AnswerCount { get; set; }
        public Nullable<int> OrderNO { get; set; }
        public Nullable<bool> ExportRecheck { get; set; }
        public string InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public string ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }
    }
}