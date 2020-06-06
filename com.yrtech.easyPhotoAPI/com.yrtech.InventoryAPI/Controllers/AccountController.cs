using System;
using System.Collections.Generic;
using System.Web.Http;
using com.yrtech.InventoryAPI.Common;
using com.yrtech.InventoryAPI.Service;
using com.yrtech.InventoryAPI.DTO;
using com.yrtech.InventoryDAL;

namespace com.yrtech.InventoryAPI.Controllers
{
    [RoutePrefix("easyPhoto/api")]
    public class AccountController : BaseController
    {
        AccountService accountService = new AccountService();
        MasterService masterService = new MasterService();

        [HttpGet]
        [Route("Account/Login")]
        public APIResult Login(string projectId,string accountId, string password)
        {
            try
            {
                List<UserInfo> accountlist = accountService.Login(projectId,accountId, password);
                if (accountlist != null && accountlist.Count != 0)
                {
                    return new APIResult() { Status = true, Body = CommonHelper.Encode(accountlist) };
                }
                else
                {
                    return new APIResult() { Status = false, Body = "用户不存在密码不匹配或账号已过期" };
                }
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
      
    }
}
