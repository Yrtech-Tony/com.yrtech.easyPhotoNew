using System.Web.Http;
using com.yrtech.InventoryAPI.Service;
using com.yrtech.InventoryAPI.Common;
using System.Collections.Generic;
using System;
using com.yrtech.InventoryDAL;
using com.yrtech.InventoryAPI.Controllers;
using com.yrtech.InventoryAPI.DTO;
using System.Net.Http;

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
        public APIResult GetShopAnswerList(string answerId,string projectId, string shopCode,string checkCode,string checkTypeId, string photoCheck, string addCheck,string key)
        {
            try
            {
                List<AnswerDto> answerList = answerService.GetShopAnswerList(answerId,projectId, shopCode, checkCode,checkTypeId,photoCheck,addCheck, key);
                foreach (AnswerDto answerDto in answerList)
                {
                    answerDto.AnswerPhotoList = answerService.GetAnswerPhotoList(answerDto.AnswerId.ToString(),"","");
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(answerList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Answer/SaveShopAnswer")]
        public APIResult SaveShopAnswer(UploadData  upload)
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
                answer.RemarkId = answerdto.RemarkId;
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
                answer = answerService.SaveShopAnswer(answer);
                foreach (AnswerPhotoDto photoDto in answerdto.AnswerPhotoList)
                {
                    AnswerPhoto photo = new AnswerPhoto();
                    photo.AnswerId = photoDto.AnswerId;
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
        [Route("Answer/AnserExport")]
        public APIResult AnserExport(string projectId, string shopCode)
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
        [Route("Master/AnswerExcelAnalysis")]
        public APIResult AnswerExcelAnalysis(string projectId, string ossPath)
        {
            try
            {
                List<AnswerDto> list = excelDataService.AnswerImport(ossPath);
                foreach (AnswerDto answer in list)
                {
                    answer.ImportChk = true;
                    answer.ImportRemark = "";
                    // 验证检查类型是否存在
                    List<CheckType> checkTypeList = masterService.GetCheckType(projectId,"",answer.CheckTypeName,true);
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
        [HttpPost]
        [Route("Master/AnswerImport")]
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
                List<AnswerPhotoDto> photoList = answerService.GetAnswerPhotoList(answerId,"","");
                return new APIResult() { Status = true, Body = CommonHelper.Encode(photoList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Answer/DeleteAnswerList")]
        public APIResult DeleteAnswerList(UploadData answer)
        {
            try
            {
                List<AnswerDto> answerList = CommonHelper.DecodeString<List<AnswerDto>>(answer.AnswerListJson);
                answerService.DeleteShopAnswer(answerList);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpGet]
        [Route("Answer/AnswerPhotoDownLoad")]
        public APIResult AnswerPhotoDownLoad(string projectId,string shopCode)
        {
            try
            {
                string downloadPath = answerService.AnswerPhotoDownLoad(projectId,shopCode);
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
