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
        public Nullable<bool> TxtChk { get; set; }// true 为输入框
        public Nullable<bool> ListShowChk { get; set; } //true为显示
        public Nullable<bool> EditChk { get; set; }// true 可编辑
    }
}