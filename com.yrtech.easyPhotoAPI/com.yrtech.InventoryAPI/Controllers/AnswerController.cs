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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectCode"></param>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Answer/DownloadAnswerList")]
        public APIResult DownloadAnswerList(string projectCode, string shopCode)
        {
            try
            {
                CommonController commonController = new CommonController();
                commonController.DownloadReport(projectCode, shopCode);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Answer/ImportAnswerList")]
        public  APIResult ImportAnswerList(UploadData answer)
        {
            try
            {
                List<ShopDto> shopDtoList = new List<ShopDto>();
                List<AnswerDto> answerList = CommonHelper.DecodeString<List<AnswerDto>>(answer.AnswerListJson);
                //foreach (AnswerDto answerDto in answerList)
                //{
                //    string brandId = masterService.GetProject("", answerDto.ProjectId.ToString(), "", "", "")[0].BrandId.ToString();
                //    GetShopInfo(brandId,"",answerDto.ShopCode,"");
                //    if (_ShopInfo != null && _ShopInfo.Count > 0)
                //    {
                //        List<AnswerDto> answerListExist = answerService.GetShopAnswerList(answerDto.ProjectId.ToString(), _ShopInfo[0].ShopId.ToString(), "", "", "", "");
                //        if (answerListExist != null && answerListExist.Count > 0)
                //        {
                //            shopDtoList.Add(_ShopInfo[0]);
                //        }
                //    }
                //}
                if (shopDtoList.Count > 0)
                {
                    string existShop = "";
                    foreach (ShopDto shop in shopDtoList)
                    {
                        existShop += shop.ShopName + ";";
                    }
                    return new APIResult() { Status = false, Body = "如下经销商已导入过，如需再次导入请先删除数据:"+existShop };
                }
                else
                {
                    answerService.ImportAnswerList(answerList[0].ProjectId.ToString(), answer.UserId, answerList);
                    return new APIResult() { Status = true, Body = "" };
                }
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
    }
}
