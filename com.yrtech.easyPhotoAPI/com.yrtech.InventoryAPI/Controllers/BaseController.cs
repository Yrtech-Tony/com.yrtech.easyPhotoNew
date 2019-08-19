using com.yrtech.InventoryAPI.Common;
using com.yrtech.InventoryAPI.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace com.yrtech.InventoryAPI.Controllers
{
    public class BaseController : ApiController
    {
        protected List<ShopDto> _ShopInfo;
        public async void GetShopInfo(string brandId, string shopId, string shopCode, string key)
        {
            string result = await CommonHelper.GetHttpClient().GetStringAsync(CommonHelper.GetAPISurveyUrl + "/Master/GetShop/" + brandId + "/" + shopId + "/" + shopCode + "/" + key);
            _ShopInfo =  CommonHelper.DecodeString<List<ShopDto>>(result);
        }
       
    }
}
