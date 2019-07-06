using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.InventoryAPI.DTO.Master
{
    [Serializable]
    public class OtherPropertyTypeDto
    {
        public int ProjectId { get; set; }
        public string OtherType { get; set; }
    }
}