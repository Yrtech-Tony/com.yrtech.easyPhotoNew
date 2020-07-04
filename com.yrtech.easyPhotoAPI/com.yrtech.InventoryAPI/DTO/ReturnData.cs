using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.yrtech.InventoryAPI.DTO
{
    /**
 * 返回数据实体类
 * @param <T>
 */
    public class ReturnData<T>
    {
        //数据集合
        public T[] rows { get; set; }
        //数据总条数
        public int total { get; set; }
        public string errcode { get; set; }
    }
}