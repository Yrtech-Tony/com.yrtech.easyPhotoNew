using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.InventoryAPI.DTO
{
    [Serializable]
    public class AnswerShopInfoDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string ShopInStatus { get; set; }
        public DateTime? ShopInDateTime { get; set; }
        public DateTime? ShopOutDateTime { get; set; }
        public int ShopId { get; set; }
       
        public int InUserId { get; set;  }
        public DateTime InDateTime { get; set; }
        public int ModifyUserId { get; set; }
        public DateTime ModifyDateTime { get; set; }
    }
}