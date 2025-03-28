﻿using com.yrtech.InventoryDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.InventoryAPI.DTO
{
    [Serializable]
    public class AnswerDto
    {
        public long AnswerId { get; set; }
        public int ProjectId { get; set; }
        public int TaskId { get; set; }
        public string TaskStatus { get; set; }
        public bool? RecheckStatus { get; set; }
        public int ShopId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string CheckCode { get; set; }
        public string CheckCodeShow { get; set; }
        public Nullable<int> CheckTypeId { get; set; }
        public string CheckTypeName { get; set; }
        public Nullable<int> RemarkId { get; set; }
        public string RemarkName { get; set; }
        public string Column1 { get; set; }
        public string Column2 { get; set; }
        public string Column3 { get; set; }
        public string Column4 { get; set; }
        public string Column5 { get; set; }
        public string Column6 { get; set; }
        public string Column7 { get; set; }
        public string Column8 { get; set; }
        public string Column9 { get; set; }
        public string Column10 { get; set; }
        public string Column11 { get; set; }
        public string Column12 { get; set; }
        public string Column13 { get; set; }
        public string Column14 { get; set; }
        public string Column15 { get; set; }
        public string Column16 { get; set; }
        public string Column17 { get; set; }
        public string Column18 { get; set; }
        public string Column19 { get; set; }
        public string Column20 { get; set; }
        public string AddCheck { get; set; }
        public string PhotoCheck { get; set; } // 是否已经拍照
        public bool? ImportChk { get; set; }
        public string ImportRemark { get; set; }
        public string ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyDateTime { get; set; }
        public string InUserID { get; set; }
        public string OpenId { get; set; }
        public Nullable<System.DateTime> InDateTime { get; set; }
        public List<AnswerPhotoDto> AnswerPhotoList { get; set; }
    }
}