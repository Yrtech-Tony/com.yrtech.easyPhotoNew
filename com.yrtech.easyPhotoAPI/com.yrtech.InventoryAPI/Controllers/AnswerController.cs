﻿using System.Web.Http;
using com.yrtech.InventoryAPI.Service;
using com.yrtech.InventoryAPI.Common;
using System.Collections.Generic;
using System;
using com.yrtech.InventoryDAL;
using com.yrtech.InventoryAPI.Controllers;
using com.yrtech.InventoryAPI.DTO;
using System.Net.Http;
using System.Linq;

namespace com.yrtech.SurveyAPI.Controllers
{

    [RoutePrefix("easyPhoto/api")]
    public class AnswerController : BaseController
    {
        AnswerService answerService = new AnswerService();
        MasterService masterService = new MasterService();
        ExcelDataService excelDataService = new ExcelDataService();
        [HttpGet]
        [Route("Answer/GetShopAnswerList")]
        public APIResult GetShopAnswerList(string answerId, string projectId, string shopCode, string checkCode, string checkTypeId,
            string photoCheck, string addCheck, string key, int offset = 0, int limit = 10000
            )
        {

            ReturnData<AnswerDto> returnData = new ReturnData<AnswerDto>();
            try
            {
                List<AnswerDto> answerList = answerService.GetShopAnswerListAll(answerId, projectId, shopCode, checkCode, checkTypeId, photoCheck, addCheck, key);
                int total = answerList.Count;
                answerList = answerList.Skip(offset).Take(limit).ToList();
                foreach (AnswerDto answerDto in answerList)
                {
                    answerDto.AnswerPhotoList = answerService.GetAnswerPhotoList(answerDto.AnswerId.ToString(), "", "");
                }

                return new APIResult() { Status = true, Total = total, Body = CommonHelper.EncodeDto<AnswerDto>(answerList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Answer/SaveShopAnswer")]
        public APIResult SaveShopAnswer(UploadData upload)
        {
            try
            {
                AnswerDto answerdto = CommonHelper.DecodeString<AnswerDto>(upload.AnswerListJson);
                Answer answer = new Answer();
                answer.AnswerId = answerdto.AnswerId;
                answer.ProjectId = answerdto.ProjectId;
                answer.ShopCode = answerdto.ShopCode;
                answer.ShopName = answerdto.ShopName;
                answer.CheckCode = answerdto.CheckCode;
                answer.CheckTypeId = answerdto.CheckTypeId;
                answer.Remark = answerdto.RemarkName;
                answer.AddCheck = answerdto.AddCheck;
                answer.ModifyUserId = answerdto.ModifyUserId;
                answer.InUserID = answerdto.InUserID;
                answer.Column1 = answerdto.Column1;
                answer.Column2 = answerdto.Column2;
                answer.Column3 = answerdto.Column3;
                answer.Column4 = answerdto.Column4;
                answer.Column5 = answerdto.Column5;
                answer.Column6 = answerdto.Column6;
                answer.Column7 = answerdto.Column7;
                answer.Column8 = answerdto.Column8;
                answer.Column9 = answerdto.Column9;
                answer.Column10 = answerdto.Column10;
                answer.Column11 = answerdto.Column11;
                answer.Column12 = answerdto.Column12;
                answer.Column13 = answerdto.Column13;
                answer.Column14 = answerdto.Column14;
                answer.Column15= answerdto.Column15;
                answer = answerService.SaveShopAnswer(answer);
                foreach (AnswerPhotoDto photoDto in answerdto.AnswerPhotoList)
                {
                    AnswerPhoto photo = new AnswerPhoto();
                    photo.AnswerId = answer.AnswerId;
                    photo.InUserId = photoDto.InUserId;
                    photo.ModifyUserId = photoDto.ModifyUserId;
                    photo.PhotoId = photoDto.PhotoId;
                    photo.PhotoUrl = photoDto.PhotoUrl;
                    answerService.SaveShopAnswerPhoto(photo);
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
        /// <param name="projectCode"></param>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Answer/AnswerExport")]
        public APIResult AnswerExport(string projectId, string shopCode)
        {
            try
            {

                return new APIResult() { Status = true, Body = CommonHelper.Encode(excelDataService.AnswerExport(projectId, shopCode)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpGet]
        [Route("Answer/AnswerExcelAnalysis")]
        public APIResult AnswerExcelAnalysis(string projectId, string ossPath)
        {
            try
            {
                List<AnswerDto> list = excelDataService.AnswerImport(ossPath);
                //list
                foreach (AnswerDto answer in list)
                {
                    answer.ImportChk = true;
                    answer.ImportRemark = "";
                    //验证检查类型是否存在
                    List<CheckType> checkTypeList = masterService.GetCheckType(projectId, "", answer.CheckTypeName, true);
                    if (checkTypeList == null || checkTypeList.Count == 0)
                    {
                        answer.ImportChk = false;
                        answer.ImportRemark += "检查类型不存在或已不使用" + ";";
                    }
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpGet]
        [Route("Answer/DeleteAnswerByShop")]
        public APIResult DeleteAnswerByShop(string projectId, string shopCode)
        {
            try
            {
                shopCode = shopCode.Replace("，", ",");
                answerService.DeleteAnswerByShop(projectId, shopCode);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpGet]
        [Route("Answer/DeleteShopAnswerPhoto")]
        public APIResult DeleteShopAnswerPhoto(string answerId, string photoId)
        {
            try
            {
                answerService.DeleteShopAnswerPhoto(answerId, photoId);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Answer/AnswerImport")]
        public APIResult AnswerImport(UploadData uploadData)
        {
            try
            {
                List<AnswerDto> list = CommonHelper.DecodeString<List<AnswerDto>>(uploadData.ListJson);
                string projectId = "";
                if (list != null && list.Count > 0) projectId = list[0].ProjectId.ToString();
                foreach (AnswerDto answer in list)
                {
                    answer.ImportChk = true;
                    answer.ImportRemark = "";
                    // 验证检查类型是否存在
                    List<CheckType> checkTypeList = masterService.GetCheckType(projectId, "", answer.CheckTypeName, true);
                    if (checkTypeList == null || checkTypeList.Count == 0)
                    {
                        answer.ImportChk = false;
                        answer.ImportRemark += "检查类型不存在或已不使用" + ";";
                    }
                    else
                    {
                        answer.CheckTypeId = checkTypeList[0].CheckTypeId;
                    }
                }
                answerService.ImportAnswerList(projectId, list);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpGet]
        [Route("Answer/GetShopAnswerPhoto")]
        public APIResult GetShopAnswerPhoto(string answerId)
        {
            try
            {
                List<AnswerPhotoDto> photoList = answerService.GetAnswerPhotoList(answerId, "", "");
                return new APIResult() { Status = true, Body = CommonHelper.Encode(photoList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        // 进店状态查询
        [HttpGet]
        [Route("Answer/GetAnswerShopInfo")]
        public APIResult GetAnswerShopInfo(string projectId, string shopCode)
        {
            try
            {
                List<AnswerShopInfoDto> list = answerService.GetAnswerShopInfo(projectId, shopCode);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpGet]
        [Route("Answer/AnswerShopInfoExport")]
        public APIResult AnswerShopInfoExport(string projectId, string shopCode)
        {
            try
            {

                return new APIResult() { Status = true, Body = CommonHelper.Encode(excelDataService.AnswerShopInfoExport(projectId, shopCode)) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpGet]
        [Route("Answer/DeleteAnswerList")]
        public APIResult DeleteAnswerList(string answerIdList)
        {
            try
            {
                string[] answerId = answerIdList.Split(',');
                answerService.DeleteShopAnswer(answerId);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }

        [HttpPost]
        [Route("Answer/SaveRecheck")]
        public APIResult SaveRecheck(Recheck recheck)
        {
            try
            {
                answerService.SaveRecheck(recheck);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Answer/AnswerPhotoDownLoad")]
        public APIResult AnswerPhotoDownLoad(string projectId, string shopCode)
        {
            try
            {
                string downloadPath = answerService.AnswerPhotoDownLoad(projectId, shopCode);
                if (string.IsNullOrEmpty(downloadPath))
                {
                    return new APIResult() { Status = false, Body = "没有可下载文件" };
                }

                return new APIResult() { Status = true, Body = CommonHelper.Encode(downloadPath) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
    }
}
