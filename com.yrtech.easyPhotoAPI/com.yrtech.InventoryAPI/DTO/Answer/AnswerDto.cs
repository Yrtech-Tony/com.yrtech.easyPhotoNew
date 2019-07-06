using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.InventoryAPI.DTO
{
    [Serializable]
    public class AnswerDto
    {
        public string AnswerId { get; set; }
        public string ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string ShopId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string VinCode { get; set; }
        public string VinCode8 { get; set; }
        public string ModelName { get; set; }
        public string SubModelName { get; set; }
        public string StockAge { get; set; }
        public string SaleFlag { get; set; }
        public string VinPhotoName { get; set; }
        public string SaleInvoicePhotoName { get; set; }
        public string CarTypePhotoName { get; set; }
        public string Remark { get; set; }
        public string AddChk { get; set; }
        public string ModifyUserId { get; set; }
        public DateTime ModifyDateTime { get; set; }
        public string InUserID { get; set; }
        public DateTime InDateTime { get; set; }
    }
}