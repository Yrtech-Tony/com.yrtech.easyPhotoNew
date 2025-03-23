using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.InventoryAPI.DTO
{
    [Serializable]
    public class AnswerPhotoDto
    {
        public long AnswerId { get; set; }
        public int PhotoId { get; set; }
        public int ProjectId { get; set; }
        public int TaskId { get; set; }
        public string CheckCode { get; set; }
        public string PhotoName { get; set; }
        public string CheckTypeName { get; set; }
        public string AddCheck { get; set;}
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string PhotoUrl { get; set; }
        public string Photo { get; set; }// 是否拍照
        public bool MustChk { get; set; }
        public string InUserId { get; set; }
        public string ModifyUserId { get; set; }
        
    }
}