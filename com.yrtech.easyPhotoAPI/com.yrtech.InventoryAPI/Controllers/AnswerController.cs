using System.Web.Http;
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
        #region 盘点码
        [HttpPost]
        [Route("Answer/SaveInventoryCode")]
        public APIResult SaveInventoryCode(InventroyCode inventroyCode)
        {
            try
            {
                List<InventroyCode> inventroyCodeList = answerService.GetInventoryCode(inventroyCode.ProjectId.ToString(), DateTime.Now.Date, DateTime.Now.AddDays(1).Date,"");
                if (inventroyCodeList != null && inventroyCodeList.Count > 0 && inventroyCodeList[0].InventroyCodeId != inventroyCode.InventroyCodeId)
                {
                    return new APIResult() { Status = false, Body = "该日期已经填写过盘库码" };
                }
                answerService.SaveInventoryCode(inventroyCode);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Answer/GetInventoryCode")]
        public APIResult GetInventoryCode(string projectId,DateTime? startDate,DateTime? endDate,string code)
        {
            try
            {
                if (endDate != null)
                {
                    endDate = Convert.ToDateTime(endDate).AddDays(1);
                }
                List<InventroyCode> list = answerService.GetInventoryCode(projectId,startDate, endDate,code);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(list) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 任务审核
        [HttpPost]
        [Route("Answer/SaveTaskStatus")]
        public APIResult SaveTaskStatus(TaskStatus taskStatus)
        {
            try
            {
                answerService.SaveTaskStatus(taskStatus);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Answer/GetTaskStatus")]
        public APIResult GetTaskStatus(string taskId,string statusCode)
        {
            try
            {
                List<TaskStatusDto> tasStatuskList = answerService.GetTaskStatus(taskId, statusCode);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(tasStatuskList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        #endregion
        #region 清单管理
        [HttpGet]
        [Route("Answer/GetTask")]
        public APIResult GetTask(string projectId, string status, string key, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (endDate != null) // 结束日期传过来的是0:00:00，所以往后延1天查询数据
                {
                    endDate = Convert.ToDateTime(endDate).AddDays(1);
                }
                List<TaskDto> taskList = answerService.GetTask("","",projectId,"","",status,key, startDate, endDate);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(taskList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Answer/GetTaskForMobile")]
        public APIResult GetTaskForMobile(string projectId,string shopCode, string status)
        {
            try
            {
                List<TaskDto> taskList = answerService.GetTaskForMobile(projectId,shopCode,status);
                List<InventroyCode> inventoryCodeList = answerService.GetInventoryCode(projectId,DateTime.Now.Date, DateTime.Now.AddDays(1).Date, "");
                if (inventoryCodeList != null && inventoryCodeList.Count > 0)
                {
                    foreach (TaskDto task in taskList)
                    {
                        task.InventoryDayCode = inventoryCodeList[0].InventoryDayCode;
                    }
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(taskList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Answer/SaveTask")]
        public APIResult SaveTask(Task task)
        {
            try
            {
                if (string.IsNullOrEmpty(task.TaskCode))
                {
                    return new APIResult() { Status = false, Body = "任务代码不能为空" };
                }
                if (string.IsNullOrEmpty(task.TaskName))
                {
                    return new APIResult() { Status = false, Body = "任务名称不能为空" };
                }
                if (task.StartDate == null) {
                    return new APIResult() { Status = false, Body = "任务开始日期不能为空" };
                }
                if (task.ExpireDateTime == null)
                {
                    return new APIResult() { Status = false, Body = "任务结束日期不能为空" };
                }
                List<TaskDto> taskList = answerService.GetTask("","",task.ProjectId.ToString(),"",task.TaskCode,"","",null,null);
                if (taskList != null && taskList.Count > 0 && taskList[0].TaskId != task.TaskId)
                {
                    return new APIResult() { Status = false, Body = "任务代码重复" };
                }
                // 开始日期0点开始
                task.StartDate = Convert.ToDateTime(Convert.ToDateTime(task.StartDate).Date.ToString("yyyy-MM-dd") + " " + "00:00:00");
                // 结束日期12点结束
                task.ExpireDateTime = Convert.ToDateTime(Convert.ToDateTime(task.ExpireDateTime).Date.ToString("yyyy-MM-dd") + " " + "23:59:59");
                answerService.SaveTask(task);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Answer/GetShopAnswerList")]
        public APIResult GetShopAnswerList(string answerId, string taskId, string shopCode, string checkCode, string checkTypeId,
           string photoCheck, string addCheck)
        {
            try
            {
                List<AnswerDto> answerList = answerService.GetShopAnswerListAll(answerId, taskId, shopCode, checkCode, checkTypeId, photoCheck, addCheck, "");
                //int total = answerList.Count;
               // answerList = answerList.Skip(offset).Take(limit).ToList();
               // 后台管理和小城查询清单时，taskId都是必须传的，所有TaskId唯一，按照TaskId查询清单所有的照片
               List<AnswerPhotoDto> photoList = answerService.GetAnswerPhotoList("",taskId,"");
                // 获取清单对应的任务的状态
                List<TaskStatusDto> taskList = answerService.GetTaskStatus(taskId, "S1");
                foreach (AnswerDto answerDto in answerList)
                {
                    answerDto.AnswerPhotoList = photoList.Where(x => x.AnswerId == answerDto.AnswerId).ToList();
                    if (taskList != null && taskList.Count > 0)
                    {
                        answerDto.TaskStatus = "已提交";
                    }
                    else {
                        answerDto.TaskStatus = "待提交";
                    }
                }
                // 获取清单对应的任务的状态
               
                
                return new APIResult() { Status = true,  Body = CommonHelper.EncodeDto<AnswerDto>(answerList) };
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
                answer.TaskId = answerdto.TaskId;
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
                answer.Column16 = answerdto.Column16;
                answer.Column17 = answerdto.Column17;
                answer.Column18 = answerdto.Column18;
                answer.Column19 = answerdto.Column19;
                answer.Column20 = answerdto.Column20;
                answer = answerService.SaveShopAnswer(answer, answerdto.OpenId);
                foreach (AnswerPhotoDto photoDto in answerdto.AnswerPhotoList)
                {
                    AnswerPhoto photo = new AnswerPhoto();
                    photo.AnswerId = answer.AnswerId;
                    photo.InUserId = photoDto.InUserId;
                    photo.ModifyUserId = photoDto.ModifyUserId;
                    photo.PhotoId = photoDto.PhotoId;
                    photo.PhotoUrl = photoDto.PhotoUrl;
                    answerService.SaveShopAnswerPhoto(photo, answerdto.OpenId);
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
        #endregion
        #region 盘点统计
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
        #endregion
        #region 清单审核
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
        #endregion
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
