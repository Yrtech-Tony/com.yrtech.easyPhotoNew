using System.Web.Http;
using com.yrtech.InventoryAPI.Service;
using com.yrtech.InventoryAPI.Common;
using System.Collections.Generic;
using System;
using System.Linq;
using com.yrtech.InventoryDAL;
using com.yrtech.InventoryAPI.DTO;

namespace com.yrtech.InventoryAPI.Controllers
{
    [RoutePrefix("easyPhoto/api")]
    public class MasterController : BaseController
    {
        MasterService masterService = new MasterService();
        ExcelDataService excelDataService = new ExcelDataService();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/GetAppVersion")]
        public APIResult GetAppVersion()
        {
            try
            {
                List<AppVersion> versionList = masterService.GetAppVersion();
                return new APIResult() { Status = true, Body = CommonHelper.Encode(versionList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SaveAppVersion")]
        public APIResult SaveAppVersion(AppVersion appVersion)
        {
            try
            {
                masterService.SaveAppVersion(appVersion);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpGet]
        [Route("Master/GetProject")]
        public APIResult GetProject(string tenantId, string projectId, string brandId, string year, string expireDateTimeCheck)
        {
            try
            {
                List<Projects> projectList = masterService.GetProject(tenantId, projectId,"","", brandId, year, expireDateTimeCheck);
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
                if (string.IsNullOrEmpty(project.ProjectCode.Replace(" ", "").ToString()))
                {
                    return new APIResult() { Status = false, Body = "期号代码不能为空" };
                }
                if (string.IsNullOrEmpty(project.ProjectName.Replace(" ", "").ToString()))
                {
                    return new APIResult() { Status = false, Body = "期号名称不能为空" };
                }
                List<Projects> projectList_Code = masterService.GetProject(project.TenantId.ToString(),"",project.ProjectCode,"",project.BrandId.ToString(),"","");
                if (projectList_Code != null && projectList_Code.Count > 0 && projectList_Code[0].ProjectId != project.ProjectId)
                {
                    return new APIResult() { Status = false, Body = "期号代码重复" };
                }
                List<Projects> projectList_Name = masterService.GetProject(project.TenantId.ToString(), "", "", project.ProjectName, project.BrandId.ToString(), "", "");
                if (projectList_Name != null && projectList_Name.Count > 0 && projectList_Name[0].ProjectId != project.ProjectId)
                {
                    return new APIResult() { Status = false, Body = "期号名称重复" };
                }
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
        public APIResult GetUserInfo(string projectId, string key)
        {
            try
            {
                List<UserInfo> userInfoList = masterService.GetUserInfo(projectId, key);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(userInfoList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Master/CreateUserInfo")]
        public APIResult CreateUserInfo(UploadData uploadData)
        {
            try
            {
                List<UserInfo> userInfoList = CommonHelper.DecodeString<List<UserInfo>>(uploadData.ListJson);

                masterService.CreateUserInfo(userInfoList);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Master/ResetExpireDateTime")]
        public APIResult ResetExpireDateTime(string projectId, string expireDateTime)
        {
            try
            {
                masterService.ResetExpireDateTime(projectId, expireDateTime);
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
        /// <param name="userInfo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Master/SaveUserInfo")]
        public APIResult SaveUserInfo(UserInfo userInfo)
        {
            try
            {
                masterService.SaveUserInfo(userInfo);
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
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Master/UserInfoDownload")]
        public APIResult UserInfoDownload(string projectId, string key)
        {
            try
            {
                string downloadPath = excelDataService.UserInfoExport(projectId, key);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(downloadPath) };
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
        [Route("Master/GetRemark")]
        public APIResult GetRemark(string projectId,string checkTypeId, string remarkId, string addCheck, bool? useChk)
        {
            try
            {
                List<Remark> remarkList = masterService.GetRemark(projectId,checkTypeId, remarkId, addCheck, "", useChk);
                return new APIResult() { Status = true, Body = CommonHelper.Encode(remarkList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SaveRemark")]
        public APIResult SaveRemark(Remark remark)
        {
            try
            {
                //List<Remark> remarkList = masterService.GetRemark("", "", "", remark.RemarkName, null);
                //if (remarkList != null && remarkList.Count != 0 && remark.RemarkId != remarkList[0].RemarkId)
                //{
                //    return new APIResult() { Status = false, Body = "备注名称重复" };
                //}

                masterService.SaveRemark(remark);

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
        public APIResult GetPhotoList(string projectId,string checkTypeId, string photoId, string addCheck, bool? useChk)
        {
            try
            {
                List<PhotoList> photoList = masterService.GetPhotoList(projectId,checkTypeId, photoId, addCheck, "", useChk);
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
                //List<PhotoList> photoList = masterService.GetPhotoList("", "", "", photo.PhotoName, null);
                //if (photoList != null && photoList.Count != 0 && photo.PhotoId != photoList[0].PhotoId)
                //{
                //    return new APIResult() { Status = false, Body = "照片名称重复" };
                //}
                //else
                //{
                    masterService.SavePhotoList(photo);
                    return new APIResult() { Status = true, Body = "" };
                //}
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
        public APIResult GetCheckTypeList(string projectId, string checkTypeId, string checkTypeName, bool? useChk)
        {
            try
            {
                List<CheckType> checkTypeList = masterService.GetCheckType(projectId, checkTypeId, checkTypeName, useChk);
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
                if (string.IsNullOrEmpty(checkType.CheckTypeName))
                {
                    return new APIResult() { Status = false, Body = "检查类型名称不能为空" };
                }
                List<CheckType> checkTypeList = masterService.GetCheckType(checkType.ProjectId.ToString(), "", checkType.CheckTypeName, null);
                if (checkTypeList != null && checkTypeList.Count != 0 && checkTypeList[0].CheckTypeId != checkType.CheckTypeId)
                {
                    return new APIResult() { Status = false, Body = "检查类型名称重复" };
                }

                masterService.SaveCheckType(checkType);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Master/GetExtendColumnProject")]
        public APIResult GetExtendColumnProject(string projectId, string columnCode,bool? addShowChk)
        {
            try
            {
                List<ExtendColumnProjectDto> extendColumnProjectList = masterService.GetExtendColumnProject(projectId, columnCode);
                if (addShowChk == true) {
                    extendColumnProjectList = extendColumnProjectList.Where(x => x.AddShowChk == true && x.UseChk == true).ToList();
                }
                return new APIResult() { Status = true, Body = CommonHelper.Encode(extendColumnProjectList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SaveExtendColumnProject")]
        public APIResult SaveExtendColumnProject(ExtendColumnProject extendColumnProject)
        {
            try
            {
                if (string.IsNullOrEmpty(extendColumnProject.ColumnName))
                {
                    return new APIResult() { Status = false, Body = "扩展列名称不能为空" };
                }
                masterService.SaveExtendColumnProject(extendColumnProject);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("Master/GetExtendColumnProjectData")]
        public APIResult GetExtendColumnProjectData(string projectId, string columnCode)
        {
            try
            {
                List<ExtendColumnProjectDataDto> extendColumnProjectList = masterService.GetExtendColumnProjectData(projectId, columnCode, "");
                return new APIResult() { Status = true, Body = CommonHelper.Encode(extendColumnProjectList) };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }

        }
        [HttpPost]
        [Route("Master/SaveExtendColumnProjectData")]
        public APIResult SaveExtendColumnProjectData(ExtendColumnProjectData extendColumnProjectData)
        {
            try
            {
                List<ExtendColumnProjectDataDto> list = masterService.GetExtendColumnProjectData(extendColumnProjectData.ProjectId.ToString(), extendColumnProjectData.ColumnCode, extendColumnProjectData.ColumnValue);
                if (list != null && list.Count > 0)
                {
                    return new APIResult() { Status = false, Body = "数据重复" };
                }
                masterService.SaveExtendColumnProjectData(extendColumnProjectData);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
        [HttpPost]
        [Route("Master/DeleteExtendColumnProjectData")]
        public APIResult DeleteExtendColumnProjectData(ExtendColumnProjectData extendColumnProjectData)
        {
            try
            {
                masterService.DeleteExtendColumnProjectData(extendColumnProjectData);
                return new APIResult() { Status = true, Body = "" };
            }
            catch (Exception ex)
            {
                return new APIResult() { Status = false, Body = ex.Message.ToString() };
            }
        }
    }
}
