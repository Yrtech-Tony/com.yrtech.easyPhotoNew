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
        /// <param name="projectId"></param>
        /// <param name="brandId"></param>
        /// <param name="year"></param>
        /// <param name="expireDateTimeCheck"></param>
        /// <returns></returns>
        public List<Projects> GetProject(string tenantId, string projectId, string brandId, string year, string expireDateTimeCheck)
        {
            if (projectId == null) projectId = "";
            if (tenantId == null) tenantId = "";
            if (year == null) year = "";
            if (expireDateTimeCheck == null) expireDateTimeCheck = "";
            if (brandId == null) brandId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId),
                                                        new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@Year", year),
                                                        new SqlParameter("@BrandId", brandId)};
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
                findOne.ProjectCode = project.ProjectCode;
                findOne.ProjectName = project.ProjectName;
                findOne.Quarter = project.Quarter;
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
        public List<UserInfo> GetUserInfo(string projectId, string key)
        {
            if (projectId == null) projectId = "";
            if (key == null) key = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                       ,new SqlParameter("@Key",key)};
            Type t = typeof(UserInfo);
            string sql = @"SELECT *
                            FROM UserInfo A 
                            WHERE ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(key))
            {
                sql += " AND (ShopCode LIKE '%'+@Key+'%' OR ShopName LIKE '%'+@Key+'%')";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<UserInfo>().ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userinfo"></param>
        public void CreateUserInfo(List<UserInfo> userInfoList)
        {
            if (userInfoList == null || userInfoList.Count == 0) return;
            string sql = "";
            string sqlDelete = "DELETE UserInfo WHERE ProjectId = '" + userInfoList[0].ProjectId.ToString() + "'";
            SqlParameter[] para = new SqlParameter[] { };
            db.Database.ExecuteSqlCommand(sqlDelete, para);
            foreach (UserInfo userInfo in userInfoList)
            {
                sql += " INSERT INTO UserInfo VALUES('";
                sql += userInfo.ProjectId.ToString() + "','";
                sql += userInfo.ShopCode + "','";
                sql += userInfo.ShopName + "','";
                sql += Guid.NewGuid().ToString().Substring(0, 6) + "','";
                sql += userInfo.ExpireDateTime.ToString() + "','";
                sql += userInfo.InUserId.ToString() + "','";
                sql += DateTime.Now.ToString() + "','";
                sql += userInfo.ModifyUserId.ToString() + "','";
                sql += DateTime.Now.ToString() + "')";

            }
            db.Database.ExecuteSqlCommand(sql, para);
        }
        public void ResetExpireDateTime(string projectId, string expireDateTime)
        {
            string sql = @"UPDATE UserInfo SET  ExpireDateTime = '" + expireDateTime + "' WHERE  ProjectId = '" + projectId + "'";
            SqlParameter[] para = new SqlParameter[] { };
            db.Database.ExecuteSqlCommand(sql, para);
        }
        public void SaveUserInfo(UserInfo userInfo)
        {
            UserInfo findOne = db.UserInfo.Where(x => (x.Id == userInfo.Id)).FirstOrDefault();
            if (findOne == null)
            {
                userInfo.InDateTime = DateTime.Now;
                db.UserInfo.Add(userInfo);
            }
            else
            {
                findOne.ExpireDateTime = userInfo.ExpireDateTime;
                findOne.ModifyUserId = userInfo.ModifyUserId;
                findOne.Password = userInfo.Password;
                findOne.ModifyDateTime = DateTime.Now;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="checkTypeId"></param>
        /// <param name="checkTypeName"></param>
        /// <returns></returns>
        public List<CheckType> GetCheckType(string projectId, string checkTypeId, string checkTypeName, bool? useChk)
        {
            if (projectId == null) projectId = "";
            if (checkTypeId == null) checkTypeId = "";
            if (checkTypeName == null) checkTypeName = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                        , new SqlParameter("@CheckTypeId", checkTypeId)
                                                     , new SqlParameter("@CheckTypeName", checkTypeName)
                                                     };
            Type t = typeof(CheckType);
            string sql = "";
            sql = @"SELECT *
                    FROM [CheckType]
                    WHERE 1=1  ";
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
            if (useChk != null)
            {
                para = para.Concat(new SqlParameter[] { new SqlParameter("@UseChk", useChk) }).ToArray();
                sql += " AND UseChk = @UseChk";
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
                findOne.UseChk = checkType.UseChk;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="addCheck"></param>
        /// <param name="noteName"></param>
        /// <returns></returns>
        public List<Remark> GetRemark(string checkTypeId, string remarkId, string addCheck, string remarkName, bool? useChk)
        {
            if (checkTypeId == null) checkTypeId = "";
            if (addCheck == null) addCheck = "";
            if (remarkId == null) remarkId = "";
            if (remarkName == null) remarkName = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@CheckTypeId", checkTypeId)
                                                       , new SqlParameter("@RemarkId", remarkId)
                                                        , new SqlParameter("@AddCheck", addCheck)
                                                        , new SqlParameter("@RemarkName", remarkName)
                                                       };
            Type t = typeof(Remark);
            string sql = "";
            sql = @"SELECT *
                    FROM [Remark]
                    WHERE 1=1 
                    ";
            if (!string.IsNullOrEmpty(checkTypeId))
            {
                sql += " AND CheckTypeId = @CheckTypeId";
            }
            if (!string.IsNullOrEmpty(remarkId))
            {
                sql += " AND RemarkId = @RemarkId";
            }
            if (!string.IsNullOrEmpty(addCheck))
            {
                sql += " AND AddCheck = @AddCheck";
            }
            if (!string.IsNullOrEmpty(remarkName))
            {
                sql += " AND RemarkName = @RemarkName";
            }
            if (useChk != null)
            {
                para = para.Concat(new SqlParameter[] { new SqlParameter("@UseChk", useChk) }).ToArray();
                sql += " AND UseChk = @UseChk";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<Remark>().ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remark"></param>
        public void SaveRemark(Remark remark)
        {
            Remark findOne = db.Remark.Where(x => (x.RemarkId == remark.RemarkId)).FirstOrDefault();
            if (findOne == null)
            {
                remark.InDateTime = DateTime.Now;
                remark.ModifyDateTime = DateTime.Now;
                db.Remark.Add(remark);
            }
            else
            {
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = remark.ModifyUserId;
                findOne.AddCheck = remark.AddCheck;
                findOne.RemarkName = remark.RemarkName;
                findOne.UseChk = remark.UseChk;
            }
            db.SaveChanges();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="checkTypeId"></param>
        /// <param name="addCheck"></param>
        /// <param name="photoName"></param>
        /// <returns></returns>
        public List<PhotoList> GetPhotoList(string checkTypeId, string photoId, string addCheck, string photoName, bool? useChk)
        {
            if (checkTypeId == null) checkTypeId = "";
            if (photoId == null) photoId = "";
            if (addCheck == null) addCheck = "";
            if (photoName == null) photoName = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@CheckTypeId", checkTypeId)
                                                        ,new SqlParameter("@PhotoId", photoId)
                                                        , new SqlParameter("@AddCheck", addCheck)
                                                        , new SqlParameter("@PhotoName", photoName)
                                                    };
            Type t = typeof(PhotoList);
            string sql = "";
            sql = @"SELECT *
                    FROM [PhotoList]
                    WHERE 1=1   
                    ";
            if (!string.IsNullOrEmpty(checkTypeId))
            {
                sql += " AND CheckTypeId = @CheckTypeId";
            }
            if (!string.IsNullOrEmpty(photoId))
            {
                sql += " AND PhotoId = @PhotoId";
            }
            if (!string.IsNullOrEmpty(addCheck))
            {
                sql += " AND AddCheck = @AddCheck";
            }
            if (!string.IsNullOrEmpty(photoName))
            {
                sql += " AND PhotoName = @PhotoName";
            }
            if (useChk != null)
            {
                para = para.Concat(new SqlParameter[] { new SqlParameter("@UseChk", useChk) }).ToArray();
                sql += " AND UseChk = @UseChk";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<PhotoList>().ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="photoList"></param>
        public void SavePhotoList(PhotoList photoList)
        {
            PhotoList findOne = db.PhotoList.Where(x => (x.PhotoId == photoList.PhotoId)).FirstOrDefault();
            if (findOne == null)
            {
                photoList.InDateTime = DateTime.Now;
                photoList.ModifyDateTime = DateTime.Now;
                db.PhotoList.Add(photoList);
            }
            else
            {
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = photoList.ModifyUserId;
                findOne.PhotoName = photoList.PhotoName;
                findOne.UseChk = photoList.UseChk;
                findOne.AddCheck = photoList.AddCheck;
                findOne.MustChk = photoList.MustChk;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 获取当前期要显示的列
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<ExtendColumnProjectDto> GetExtendColumnProject(string projectId, string columnCode)
        {
            if (projectId == null) projectId = "";
            if (columnCode == null) columnCode = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                    ,new SqlParameter("@ColumnCode", columnCode) };
            Type t = typeof(ExtendColumnProjectDto);
            string sql = @"SELECT @ProjectId AS ProjectId,ColumnCode,
                            ISNULL((SELECT ColumnName FROM ExtendColumnProject WHERE ProjectId = @ProjectId  AND ColumnCode = A.ColumnCode),'') AS ColumnName
                            ,(SELECT AddShowChk FROM ExtendColumnProject WHERE ProjectId =  @ProjectId AND ColumnCode = A.ColumnCode) AS AddShowChk
                            ,(SELECT UseChk FROM ExtendColumnProject WHERE ProjectId =  @ProjectId AND ColumnCode = A.ColumnCode) AS UseChk
                            FROM ExtendColumn A WHERE 1=1";
            if (!string.IsNullOrEmpty(columnCode))
            {
                sql += " AND ColumnCode = @ColumnCode";
            }
            sql += "ORDER BY ColumnCode ASC";
            return db.Database.SqlQuery(t, sql, para).Cast<ExtendColumnProjectDto>().ToList();
        }
        public void SaveExtendColumnProject(ExtendColumnProject extendColumnProject)
        {
            ExtendColumnProject findOne = db.ExtendColumnProject.Where(x => (x.ProjectId == extendColumnProject.ProjectId && x.ColumnCode == extendColumnProject.ColumnCode)).FirstOrDefault();
            if (findOne == null)
            {
                extendColumnProject.InDateTime = DateTime.Now;
                extendColumnProject.ModifyDateTime = DateTime.Now;
                db.ExtendColumnProject.Add(extendColumnProject);
            }
            else
            {
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = extendColumnProject.ModifyUserId;
                findOne.ColumnName = extendColumnProject.ColumnName;
                findOne.AddShowChk = extendColumnProject.AddShowChk;
                findOne.UseChk = extendColumnProject.UseChk;
            }
            db.SaveChanges();
        }
        public List<ExtendColumnProjectDataDto> GetExtendColumnProjectData(string projectId, string columnCode, string columnValue)
        {
            if (projectId == null) projectId = "";
            if (columnCode == null) columnCode = "";
            if (columnValue == null) columnValue = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                    ,new SqlParameter("@ColumnCode", columnCode)
                                                    ,new SqlParameter("@ColumnValue", columnValue) };
            Type t = typeof(ExtendColumnProjectDataDto);
            string sql = @"SELECT A.*,B.ColumnName
                            FROM ExtendColumnProjectData A INNER JOIN ExtendColumnProject B ON A.ProjectId = B.ProjectId 
                                                                                            AND A.ColumnCode = B.ColumnCode
                            WHERE A.ProjectId = @ProjectId AND A.ColumnCode = @ColumnCode";
            if (!string.IsNullOrEmpty(columnValue))
            {
                sql += " AND ColumnValue = @ColumnValue";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<ExtendColumnProjectDataDto>().ToList();
        }
        /// <summary>
        /// 不需要更新直接插入即可
        /// </summary>
        /// <param name="extendColumnProjectData"></param>
        public void SaveExtendColumnProjectData(ExtendColumnProjectData extendColumnProjectData)
        {
            extendColumnProjectData.InDateTime = DateTime.Now;
            extendColumnProjectData.ModifyDateTime = DateTime.Now;
            db.ExtendColumnProjectData.Add(extendColumnProjectData);
            db.SaveChanges();
        }
        public void DeleteExtendColumnProjectData(ExtendColumnProjectData extendColumnProjectData)
        {
            string sql = "DELETE ExtendColumnProjectData WHERE ProjectId = '" + extendColumnProjectData.ProjectId.ToString() + "'";
            sql += " AND ColumnCode = '" + extendColumnProjectData.ColumnCode + "'";
            sql += " AND ColumnValue = '" + extendColumnProjectData.ColumnValue + "'";
            SqlParameter[] para = new SqlParameter[] { };
            db.Database.ExecuteSqlCommand(sql, para);

        }

    }
}