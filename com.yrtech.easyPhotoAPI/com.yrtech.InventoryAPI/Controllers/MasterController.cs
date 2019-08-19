using System.Web.Http;
using com.yrtech.InventoryAPI.Service;
using com.yrtech.InventoryAPI.Common;
using System.Collections.Generic;
using System;
using com.yrtech.InventoryDAL;
using com.yrtech.InventoryAPI.DTO;

namespace com.yrtech.InventoryAPI.Controllers
{
    [RoutePrefix("easyPhoto/api")]
    public class MasterController : BaseController
    {
        AnswerService answerService = new AnswerService();
        MasterService masterService = new MasterService();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetProject")]
        public APIResult GetProject(string tenantId, string projectId,string brandId, string year, string expireDateTimeCheck)
        {
            try
            {
                List<Projects> projectList = masterService.GetProject(tenantId, projectId,brandId, year, expireDateTimeCheck);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(projectList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Master/SaveProject")]
        public APIResult SaveProject(Projects project)
        {
            try
            {
                masterService.SaveProjects(project);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="accountId"></param>
        /// <param name="telNo"></param>
        /// <param name="expireDateTimeCheck"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetUserInfo")]
        public APIResult GetUserInfo(string tenantId, string accountId, string telNo, string expireDateTimeCheck, string key)
        {
            try
            {
                List<UserInfo> userInfoList = masterService.GetUserInfo(tenantId, key, accountId, telNo, expireDateTimeCheck);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(userInfoList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Master/SaveUserInfo")]
        public APIResult SaveUserInfo(UserInfo userInfo)
        {
            try
            {
                List<UserInfo> userInfoList_accountId = masterService.GetUserInfo(userInfo.TenantId.ToString(), "", userInfo.AccountId, "", "");
                List<UserInfo> userInfoList_TelNO = masterService.GetUserInfo(userInfo.TenantId.ToString(), "", "", userInfo.TelNO, "");
                // 验证账号是否重复
                if (userInfoList_accountId != null && userInfoList_accountId.Count != 0)
                {
                    return new APIResult() { Status = false, Body = "账号重复，请使用其他账号" };
                }// 验证手机号是否重复
                else if (userInfoList_TelNO != null && userInfoList_TelNO.Count != 0)
                {
                    return new APIResult() { Status = false, Body = "手机号重复" };
                }
                else
                {
                    masterService.SaveUserInfo(userInfo);
                    return new APIResult() { Status = true, Body = "" };
                }
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Master/SaveUserInfoShop")]
        public APIResult SaveUserInfoShop(UploadData uploadData)
        {
            try
            {
                List<UserInfoShop> userInfoList = CommonHelper.DecodeString<List<UserInfoShop>>(uploadData.AnswerListJson);
                foreach (UserInfoShop userInfoShop in userInfoList)
                {
                    masterService.SaveUserInfoShop(userInfoShop);
                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Master/DeleteUserInfoShop")]
        public APIResult DeleteUserInfoShop(UploadData uploadData)
        {
            try
            {
                List<UserInfoShop> userInfoList = CommonHelper.DecodeString<List<UserInfoShop>>(uploadData.AnswerListJson);
                foreach (UserInfoShop userInfoShop in userInfoList)
                {
                    masterService.DeleteUserInfoShop(userInfoShop);
                }
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="shopCode"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetUserInfoShop")]
        public APIResult GetUserInfoShop(string projectId, string shopId, string key, string userId)
        {
            try
            {
                List<UserInfoShop> shopList = masterService.GetUserInfoShop(projectId, shopId, key, userId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(shopList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="checkTypeId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetNote")]
        public APIResult GetNote(string projectId, string checkTypeId, string addCheck)
        {
            try
            {
                List<Note> noteList = masterService.GetNote(projectId, checkTypeId, addCheck,""); 
                return new APIResult() { Status = true, Body = CommonHelper.Encode(noteList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SaveNote")]
        public APIResult SaveNote(Note note)
        {
            try
            {
                List<Note> noteList = masterService.GetNote(note.ProjectId.ToString(), note.CheckTypeId.ToString(), "", note.NoteName);
                if (noteList != null && noteList.Count != 0)
                {
                    return new APIResult() { Status = false, Body = "备注名称重复" };
                }
                else
                {
                    masterService.SaveNote(note);
                }

                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Master/GetOtherPropertyType")]
        public APIResult GetOtherPropertyType(string projectId)
        {
            try
            {
                List<OtherPropertyTypeDto> otherPropertyTypeList = masterService.GetOtherPropertyType(projectId); ;
                return new APIResult() { Status = true, Body = CommonHelper.Encode(otherPropertyTypeList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="otherType"></param>
        /// <param name="otherCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetOtherProperty")]
        public APIResult GetOtherProperty(string projectId, string otherType, string otherCode)
        {
            try
            {
                List<OtherProperty> otherPropertyList = masterService.GetOtherProperty(projectId, otherType, otherCode,"");
                return new APIResult() { Status = true, Body = CommonHelper.Encode(otherPropertyList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SaveOtherProperty")]
        public APIResult SaveOtherProperty(OtherProperty otherProperty)
        {
            try
            {
                List<OtherProperty> otherPropertyList = masterService.GetOtherProperty(otherProperty.ProjectId.ToString(),otherProperty.OtherType,otherProperty.OtherCode,otherProperty.OtherName);
                if (otherPropertyList != null && otherPropertyList.Count != 0)
                {
                    return new APIResult() { Status = false, Body = "其他属性名称重复" };
                }
                else
                {
                    masterService.SaveOtherProperty(otherProperty);
                }

                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="checkTypeId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetPhotoList")]
        public APIResult GetPhotoList(string projectId, string checkTypeId, string addCheck)
        {
            try
            {
                List<PhotoList> photoList = masterService.GetPhotoList(projectId, checkTypeId, addCheck,"");
                return new APIResult() { Status = true, Body = CommonHelper.Encode(photoList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Master/SavePhotoList")]
        public APIResult SavePhotoList(PhotoList photo)
        {
            try
            {
                List<PhotoList> photoList = masterService.GetPhotoList(photo.ProjectId.ToString(), photo.CheckTypeId.ToString(), "", photo.PhotoName);
                if (photoList != null && photoList.Count != 0)
                {
                    return new APIResult() { Status = false, Body = "照片名称重复" };
                }
                else
                {
                    masterService.SavePhotoList(photo);
                    return new APIResult() { Status = true, Body = "" };
                }
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="checkTypeId"></param>
        /// <param name="addCheck"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetCheckTypeList")]
        public APIResult GetCheckTypeList(string projectId, string checkTypeId, string checkTypeName)
        {
            try
            {
                List<CheckType> checkTypeList = masterService.GetCheckType(projectId, checkTypeId, checkTypeName);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(checkTypeList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkType"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Master/SaveCheckType")]
        public APIResult SaveCheckType(CheckType checkType)
        {
            try
            {
                List<CheckType> checkTypeList = masterService.GetCheckType(checkType.ProjectId.ToString(), "", checkType.CheckTypeName);
                if (checkTypeList != null && checkTypeList.Count != 0)
                {
                    return new APIResult() { Status = false, Body = "检查类型重复" };
                }
                else
                {
                    masterService.SaveCheckType(checkType);
                    return new APIResult() { Status = true, Body = "" };
                }
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
    }
}
