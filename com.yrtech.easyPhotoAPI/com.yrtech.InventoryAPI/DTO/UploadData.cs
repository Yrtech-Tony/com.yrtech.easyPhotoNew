using com.yrtech.SurveyAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.InventoryAPI.DTO
{
    public class UploadData
    {
        public string UserId { get; set; }
        public string AnswerListJson { get; set; }
        public string AnswerShopInfoListJson { get; set; }
        public string AnswerShopConsultantListJson { get; set; }
        public string ListJson { get; set; }
        
    }
}