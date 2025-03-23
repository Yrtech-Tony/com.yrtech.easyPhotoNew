using com.yrtech.InventoryDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.InventoryAPI.DTO
{
    [Serializable]
    public class TaskStatusDto
    {
        public int StatusId { get; set; }
        public Nullable<int> TaskId { get; set; }
        public string TaskCode { get; set; }
        public string TaskName { get; set; }
        public string ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public string SignUrl { get; set; }
        public Nullable<int> InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
    }
}