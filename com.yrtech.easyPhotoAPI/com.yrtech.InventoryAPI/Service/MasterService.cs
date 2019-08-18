using com.yrtech.InventoryAPI.DTO;
using com.yrtech.InventoryDAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace com.yrtech.InventoryAPI.Service
{
    public class MasterService
    {
        com.yrtech.InventoryDAL.InventoryDAL db = new InventoryDAL.InventoryDAL();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<Projects> GetProject(string tenantId, string projectId,string brandId, string year, string expireDateTimeCheck)
        {
            if (tenantId == null) tenantId = "";
            if (projectId == null) projectId = "";
            if (year == null) year = "";
            if (expireDateTimeCheck == null) expireDateTimeCheck = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId),
                                                        new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@Year", year),new SqlParameter("@BrandId", brandId)};
            Type t = typeof(Projects);
            string sql = "";
            sql = @"SELECT * FROM Projects
                    WHERE 1=1
                    ";
            if (!string.IsNullOrEmpty(tenantId))
            {
                sql += " AND TenantId = @TenantId";
            }
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(brandId))
            {
                sql += " AND BrandId = @BrandId";
            }
            if (!string.IsNullOrEmpty(year))
            {
                sql += " AND Year = @Year";
            }
            if (expireDateTimeCheck == "N") // N:没有过期的，不传查询全部
            {
                sql += " AND GETDATE()<ExpireDateTime";
            }
            else if (expireDateTimeCheck == "Y")
            {
                sql += " AND GETDATE()>ExpireDateTime";
            }

            sql += " ORDER BY OrderNO DESC";
            return db.Database.SqlQuery(t, sql, para).Cast<Projects>().ToList();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        public void SaveProjects(Projects project)
        {
            Projects findOne = db.Projects.Where(x => (x.ProjectId == project.ProjectId)).FirstOrDefault();
            if (findOne == null)
            {
                project.InDateTime = DateTime.Now;
                db.Projects.Add(project);
            }
            else
            {
                findOne.BrandId = project.BrandId;
                findOne.BrandName = project.BrandName;
                findOne.ExpireDateTime = project.ExpireDateTime;
                findOne.ModifyUserId = project.ModifyUserId;
                findOne.OrderNO = project.OrderNO;
                findOne.OtherPropertyShow = project.OtherPropertyShow;
                findOne.ProjectCode = project.ProjectCode;
                findOne.ProjectName = project.ProjectName;
                findOne.Quarter = project.Quarter;
                findOne.ScoreShow = project.ScoreShow;
                findOne.Year = project.Year;
                findOne.ModifyDateTime = DateTime.Now;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="accountId"></param>
        /// <param name="accountName"></param>
        /// <param name="expireDateCheck">N:未过期</param>
        /// <returns></returns>
        public List<UserInfo> GetUserInfo(string tenantId, string key, string accountId, string telNo, string expireDateTimeCheck)
        {
            if (tenantId == null) tenantId = "";
            if (key == null) key = "";
            if (expireDateTimeCheck == null) expireDateTimeCheck = "";
            if (accountId == null) accountId = "";
            if (telNo == null) telNo = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId)
                                                       ,new SqlParameter("@Key",key)
                                                       ,new SqlParameter("@ExpireDateTimeCheck",expireDateTimeCheck)
                                                       ,new SqlParameter("@TelNO",telNo)
                                                       ,new SqlParameter("@AccountId",accountId)};
            Type t = typeof(UserInfo);
            string sql = @"SELECT *
                            FROM UserInfo A 
                            WHERE TenantId = @TenantId";
            if (expireDateTimeCheck == "N")// N:没有过期的，不传查询全部
            {
                sql += " AND GETDATE<ExpireDateTime";
            }
            else if (expireDateTimeCheck == "Y")
            {
                sql += " AND GETDATE>ExpireDateTime";
            }
            if (!string.IsNullOrEmpty(key))
            {
                sql += " AND (AccountName LIKE '%'+@Key+'%' OR AccontId LIKE '%'+@Key+'%' OR Email LIKE '%'+@Key+'%' OR TelNo LIKE '%'+@Key+'%'";
            }
            if (!string.IsNullOrEmpty(telNo))
            {
                sql += " AND TelNo = @TelNO";
            }
            if (!string.IsNullOrEmpty(accountId))
            {
                sql += " AND AccountId = @AccountId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<UserInfo>().ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userinfo"></param>
        public void SaveUserInfo(UserInfo userinfo)
        {
            UserInfo findOne = db.UserInfo.Where(x => (x.Id == userinfo.Id)).FirstOrDefault();
            if (findOne == null)
            {
                userinfo.InDateTime = DateTime.Now;
                db.UserInfo.Add(userinfo);
            }
            else
            {
                findOne.AccountId = userinfo.AccountId;
                findOne.AccountName = userinfo.AccountName;
                findOne.Email = userinfo.Email;
                findOne.ExpireDateTime = userinfo.ExpireDateTime;
                findOne.HeadPicUrl = userinfo.HeadPicUrl;
                findOne.InDateTime = DateTime.Now;
                findOne.InUserId = userinfo.InUserId;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = userinfo.ModifyUserId;
                findOne.Password = userinfo.Password;
                findOne.TelNO = userinfo.TelNO;
                findOne.UseChk = userinfo.UseChk;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="shopCode"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public List<UserInfoShop> GetUserInfoShop(string projectId, string shopId, string key, string userId)
        {
            if (key == null) key = "";
            if (shopId == null) shopId = "";
            if (projectId == null) projectId = "";
            if (userId == null) userId = "";
            SqlParameter[] para = new SqlParameter[] {new SqlParameter("@ProjectId", projectId),
                                                    new SqlParameter("@ShopId", shopId),
                                                    new SqlParameter("@Key", key),
                                                    new SqlParameter("@UserId", userId)};
            Type t = typeof(UserInfoShop);
            string sql = "";
            sql = @"SELECT 
                       *
                      FROM [UserInfoShop]
                    WHERE  1=1 ";

            if (!string.IsNullOrEmpty(key))
            {
                sql += " AND (ShopCode LIKE '%'+@Key+'%' OR AccountId LIKE '%'+@Key+'%')";
            }
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND ShopId = @ShopId";
            }
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(userId))
            {
                sql += " AND UserId= @UserId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<UserInfoShop>().ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userInfoShop"></param>
        public void SaveUserInfoShop(UserInfoShop userInfoShop)
        {
            UserInfoShop findOne = db.UserInfoShop.Where(x => (x.UserId == userInfoShop.UserId && x.ShopId == userInfoShop.ShopId && x.ProjectId == userInfoShop.ProjectId)).FirstOrDefault();
            if (findOne == null)
            {
                userInfoShop.InDateTime = DateTime.Now;
                db.UserInfoShop.Add(userInfoShop);
            }
            else
            {
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = userInfoShop.ModifyUserId;
                findOne.ShopCode = userInfoShop.ShopCode;
                findOne.ShopName = userInfoShop.ShopName;
                findOne.AccountId = userInfoShop.AccountId;
                findOne.AccountName = userInfoShop.AccountName;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="checkTypeId"></param>
        /// <returns></returns>
        public List<Note> GetNote(string projectId, string checkTypeId, string addCheck,string noteName)
        {
            if (projectId == null) projectId = "";
            if (checkTypeId == null) checkTypeId = "";
            if (addCheck == null) addCheck = "";
            if (noteName == null) noteName = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                        , new SqlParameter("@CheckTypeId", checkTypeId)
                                                        , new SqlParameter("@AddCheck", addCheck)
                                                        , new SqlParameter("@NoteName", noteName)};
            Type t = typeof(Note);
            string sql = "";
            sql = @"SELECT *
                    FROM [Note]
                    WHERE 1=1   
                    ";
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(checkTypeId))
            {
                sql += " AND CheckTypeId = @CheckTypeId";
            }
            if (!string.IsNullOrEmpty(addCheck))
            {
                sql += " AND AddCheck = @AddCheck";
            }
            if (!string.IsNullOrEmpty(noteName))
            {
                sql += " AND NoteName = @NoteName";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<Note>().ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="note"></param>
        public void SaveNote(Note note)
        {
            Note findOne = db.Note.Where(x => (x.NoteID==note.NoteID)).FirstOrDefault();
            if (findOne == null)
            {
                note.InDateTime = DateTime.Now;
                db.Note.Add(note);
            }
            else
            {
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = note.ModifyUserId;
                findOne.AddCheck=note.AddCheck;
                findOne.CheckTypeId =note.CheckTypeId;
                findOne.NoteName = note.NoteName;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="checkTypeId"></param>
        /// <returns></returns>
        public List<CheckType> GetCheckType(string projectId, string checkTypeId, string checkTypeName)
        {
            if (projectId == null) projectId = "";
            if (checkTypeId == null) checkTypeId = "";
            if (checkTypeName == null) checkTypeName = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                        , new SqlParameter("@CheckTypeId", checkTypeId)
                                                     , new SqlParameter("@CheckTypeName", checkTypeName)};
            Type t = typeof(CheckType);
            string sql = "";
            sql = @"SELECT *
                    FROM [CheckType]
                    WHERE 1=1   
                    ";
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(checkTypeId))
            {
                sql += " AND CheckTypeId = @CheckTypeId";
            }
            if (!string.IsNullOrEmpty(checkTypeName))
            {
                sql += " AND CheckTypeName = @CheckTypeName";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<CheckType>().ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkType"></param>
        public void SaveCheckType(CheckType checkType)
        {
            CheckType findOne = db.CheckType.Where(x => (x.CheckTypeId == checkType.CheckTypeId)).FirstOrDefault();
            if (findOne == null)
            {
                checkType.InDateTime = DateTime.Now;
                db.CheckType.Add(checkType);
            }
            else
            {
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = checkType.ModifyUserId;
                findOne.CheckTypeName = checkType.CheckTypeName;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="otherType"></param>
        /// <param name="otherCode"></param>
        /// <returns></returns>
        public List<OtherProperty> GetOtherProperty(string projectId, string otherType, string otherCode,string otherName)
        {
            if (projectId == null) projectId = "";
            if (otherType == null) otherType = "";
            if (otherCode == null) otherCode = "";
            if (otherName == null) otherName = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                        , new SqlParameter("@OtherType", otherType)
                                                        , new SqlParameter("@OtherCode", otherCode)
                                                    , new SqlParameter("@OtherName", otherName)};
            Type t = typeof(OtherProperty);
            string sql = "";
            sql = @"SELECT *
                    FROM [OtherProperty]
                    WHERE 1=1   
                    ";
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(otherType))
            {
                sql += " AND OtherType = @OtherType";
            }
            if (!string.IsNullOrEmpty(otherCode))
            {
                sql += " AND OtherCode = @OtherCode";
            }
            if (!string.IsNullOrEmpty(otherName))
            {
                sql += " AND OtherName = @OtherName";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<OtherProperty>().ToList();
        }
        public void SaveOtherProperty(OtherProperty otherProperty)
        {
            OtherProperty findOne = db.OtherProperty.Where(x => (x.ProjectId==otherProperty.ProjectId&&x.OtherType==otherProperty.OtherType&&x.OtherCode==otherProperty.OtherCode)).FirstOrDefault();
            if (findOne == null)
            {
                otherProperty.InDateTime = DateTime.Now;
                db.OtherProperty.Add(otherProperty);
            }
            else
            {
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = otherProperty.ModifyUserId;
                findOne.OtherName = otherProperty.OtherName;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<OtherPropertyTypeDto> GetOtherPropertyType(string projectId)
        {
            if (projectId == null) projectId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId) };
            Type t = typeof(OtherPropertyTypeDto);
            string sql = "";
            sql = @"SELECT DISTINCT ProjectId,OtherType
                    FROM [OtherProperty]
                    WHERE 1=1   
                    ";
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND ProjectId = @ProjectId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<OtherPropertyTypeDto>().ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="checkTypeId"></param>
        /// <returns></returns>
        public List<PhotoList> GetPhotoList(string projectId, string checkTypeId, string addCheck,string photoName)
        {
            if (projectId == null) projectId = "";
            if (checkTypeId == null) checkTypeId = "";
            if (addCheck == null) addCheck = "";
            if (photoName == null) photoName = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                        , new SqlParameter("@CheckTypeId", checkTypeId)
                                                        , new SqlParameter("@AddCheck", addCheck)
                                                        , new SqlParameter("@PhotoName", photoName)};
            Type t = typeof(PhotoList);
            string sql = "";
            sql = @"SELECT *
                    FROM [PhotoList]
                    WHERE 1=1   
                    ";
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(checkTypeId))
            {
                sql += " AND CheckTypeId = @CheckTypeId";
            }
            if (!string.IsNullOrEmpty(addCheck))
            {
                sql += " AND AddCheck = @AddCheck";
            }
            if (!string.IsNullOrEmpty(photoName))
            {
                sql += " AND PhotoName = @PhotoName";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<PhotoList>().ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="photoList"></param>
        public void SavePhotoList(PhotoList photoList)
        {
            PhotoList findOne = db.PhotoList.Where(x => (x.PhotoId==photoList.PhotoId)).FirstOrDefault();
            if (findOne == null)
            {
                photoList.InDateTime = DateTime.Now;
                db.PhotoList.Add(photoList);
            }
            else
            {
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = photoList.ModifyUserId;
                findOne.PhotoName = photoList.PhotoName;
            }
            db.SaveChanges();
        }
    }
}