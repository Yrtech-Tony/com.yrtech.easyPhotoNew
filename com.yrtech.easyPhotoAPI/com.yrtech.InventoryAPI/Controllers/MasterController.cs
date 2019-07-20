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
    public class MasterController : ApiController
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
        public APIResult GetProject(string tenantId,string projectId,string year,string expireDateTimeCheck)
        {
            try
            {
                List<Projects> projectList = masterService.GetProject(tenantId,projectId,year, expireDateTimeCheck);
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
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="shopCode"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetUserInfoShop")]
        public APIResult GetUserInfoShop(string projectId,string shopId,string shopCode,string userId)
        {
            try
            {
                List<UserInfoShop> shopList = masterService.GetUserInfoShop(projectId,shopId,shopCode,userId); 
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
        public APIResult GetNote(string projectId, string checkTypeId,string addCheck)
        {
            try
            {
                List<Note> noteList = masterService.GetNote(projectId,checkTypeId, addCheck); ;
                return new APIResult() { Status = true, Body = CommonHelper.Encode(noteList) };
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
        public APIResult GetOtherProperty(string projectId, string otherType,string otherCode)
        {
            try
            {
                List<OtherProperty> otherPropertyList = masterService.GetOtherProperty(projectId,otherType,otherCode); 
                return new APIResult() { Status = true, Body = CommonHelper.Encode(otherPropertyList) };
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
        public APIResult GetPhotoList(string projectId, string checkTypeId)
        {
            try
            {
                List<PhotoList> otherPropertyList = masterService.GetPhotoList(projectId,checkTypeId);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(otherPropertyList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
    }
}
