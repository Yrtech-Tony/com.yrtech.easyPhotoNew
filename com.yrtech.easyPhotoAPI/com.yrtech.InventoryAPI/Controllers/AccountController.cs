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
        AnswerService answerService = new AnswerService(); 
        [HttpGet]
        [Route("Account/Login")]
        public APIResult Login(string projectId,string accountId, string password)
        {
            try
            {
                List<UserInfoDto> accountlist = accountService.Login(projectId,accountId, password);
                if (accountlist != null && accountlist.Count != 0)
                {
                    // 临时处理问题添加代码，后期修改app
                    List<HiddenColumn> ossInfoList = masterService.GetHiddenCode("OSS信息", "");
                    foreach (HiddenColumn ossInfo in ossInfoList)
                    {
                        if (ossInfo.HiddenCode == "EndPoint")
                        {
                            ossInfo.HiddenName = ossInfo.HiddenName.Replace("https", "http");
                        }
                    }
                    accountlist[0].OSSInfo = ossInfoList;
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

        [HttpGet]
        [Route("Account/LoginForMobile")]
        public APIResult LoginForMobile(string accountId, string password,string openId,string telNO)
        {
            try
            {
                List<UserInfoDto> accountlist = accountService.LoginForMobile(accountId, password);
                if (accountlist != null && accountlist.Count != 0)
                {
                    List<HiddenColumn> ossInfoList = masterService.GetHiddenCode("OSS信息", "");
                    accountlist[0].OSSInfo = ossInfoList;
                    string endPoint = "";
                    string bucket = "";
                    foreach (HiddenColumn hiddenColumn in ossInfoList)
                    {
                        if (hiddenColumn.HiddenCode == "EndPoint")
                        {
                            endPoint = hiddenColumn.HiddenName;
                        }
                        if (hiddenColumn.HiddenCode == "Bucket")
                        {
                            bucket = hiddenColumn.HiddenName;
                        }
                    }
                    accountlist[0].OSSBaseUrl = endPoint.Insert(8, bucket + ".");
                    accountlist[0].OSSInfo = ossInfoList;
                    accountlist[0].OpenId = openId;
                    // 小程序登录时绑定OpenId和TelNO
                    if (!string.IsNullOrEmpty(openId))
                    {
                        // 更新UserId、OpenId、电话的映射关系
                        UserInfoOpenId userInfoOpenId = new UserInfoOpenId();
                        userInfoOpenId.UserId = accountlist[0].Id;
                        userInfoOpenId.OpenId = openId;
                        userInfoOpenId.TelNO = telNO;
                        accountService.UserIdOpenIdSave(userInfoOpenId);
                    }

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
