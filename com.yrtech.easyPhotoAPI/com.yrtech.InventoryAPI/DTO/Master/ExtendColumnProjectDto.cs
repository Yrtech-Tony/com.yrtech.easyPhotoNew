using System;

namespace com.yrtech.InventoryAPI.DTO
{
    [Serializable]
    public class ExtendColumnProjectDto
    {
        public string ProjectId { get; set; }
        public string ColumnCode { get; set; }
        public string ColumnName { get; set; }
        public Nullable<bool> UseChk { get; set; }
        public Nullable<bool> AddShowChk { get; set; }
    }
}