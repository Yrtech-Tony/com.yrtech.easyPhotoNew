using System;

namespace com.yrtech.InventoryAPI.DTO
{
    
    public class ExtendColumnProjectDataDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ColumnCode { get; set; }
        public string ColumnName { get; set; }
        public string ColumnValue { get; set; }
        public string InUserId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public string ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }
    }
}