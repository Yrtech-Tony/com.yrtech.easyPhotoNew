using System.Web.Http;
using com.yrtech.InventoryAPI.Service;
using com.yrtech.InventoryAPI.Common;
using System.Collections.Generic;
using System;
using com.yrtech.InventoryDAL;
using com.yrtech.InventoryAPI.Controllers;
using com.yrtech.InventoryAPI.DTO;

namespace com.yrtech.SurveyAPI.Controllers
{

    [RoutePrefix("easyPhoto/api")]
    public class AnswerController : ApiController
    {
        AnswerService answerService = new AnswerService();
        MasterService masterService = new MasterService();
        [HttpGet]
        [Route("Answer/GetShopAnswerList")]
        public APIResult GetShopAnswerList(string projectId, string shopId,string checkCode,string checkTypeId, string photoCheck, string addCheck)
        {
            try
            {
                List<AnswerDto> answerList = answerService.GetShopAnswerList(projectId,shopId,checkCode,checkTypeId,photoCheck,addCheck);
                foreach (AnswerDto answerDto in answerList)
                {
                    answerDto.answerPhotoList = answerService.GetAnswerPhotoList(answerDto.AnswerId.ToString());
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
        public APIResult SaveShopAnswer(UploadData  answer)
        {
            try
            {
                AnswerDto answerdto = CommonHelper.DecodeString<AnswerDto>(answer.AnswerListJson);
                answerService.SaveShopAnswer(answerdto);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Answer/DownloadAnswerList")]
        public APIResult DownloadAnswerList(string projectCode, string shopCode)
        {
            try
            {
                CommonController commonController = new CommonController();
                //commonController.DownloadReport(projectCode, shopCode);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
    }
}
