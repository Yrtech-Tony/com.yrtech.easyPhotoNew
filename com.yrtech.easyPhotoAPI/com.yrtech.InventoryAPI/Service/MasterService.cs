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
        // 获取Survey 的 HiddenColum
        public List<HiddenColumn> GetHiddenCode(string hiddenCodeGroup, string hiddenCode)
        {

            if (hiddenCodeGroup == null) hiddenCodeGroup = "";
            if (hiddenCode == null) hiddenCode = "";
            Type t = typeof(HiddenColumn);
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@HiddenCodeGroup", hiddenCodeGroup),
                                                        new SqlParameter("@HiddenCode", hiddenCode) };
            string sql = "";
            sql = @"SELECT *
                   FROM [com.yrtech.survey].[dbo].[HiddenColumn] WHERE 1=1";
            if (!string.IsNullOrEmpty(hiddenCodeGroup))
            {
                sql += " AND HiddenCodeGroup = @HiddenCodeGroup";
            }
            if (!string.IsNullOrEmpty(hiddenCode))
            {
                sql += " AND HiddenCode = @HiddenCode";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<HiddenColumn>().ToList();
        }
        public List<AppVersion> GetAppVersion()
        {
            SqlParameter[] para = new SqlParameter[] { };
            Type t = typeof(AppVersion);
            string sql = @"SELECT *
                            FROM AppVersion A ";
            return db.Database.SqlQuery(t, sql, para).Cast<AppVersion>().ToList();
        }
        public void SaveAppVersion(AppVersion appVersion)
        {
            string sql = "DELETE AppVersion ";
            sql += " INSERT INTO AppVersion VALUES('" + appVersion.Version + "',GETDATE())";
            SqlParameter[] para = new SqlParameter[] { };
            db.Database.ExecuteSqlCommand(sql, para);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="brandId"></param>
        /// <param name="year"></param>
        /// <param name="expireDateTimeCheck"></param>
        /// <returns></returns>
        public List<Projects> GetProject(string tenantId, string projectId, string projectCode, string projectName, string brandId, string year, string expireDateTimeCheck)
        {

            if (projectId == null) projectId = "";
            if (tenantId == null) tenantId = "";
            if (year == null) year = "";
            if (expireDateTimeCheck == null) expireDateTimeCheck = "";
            if (projectCode == null) projectCode = "";
            if (projectName == null) projectName = "";
            if (brandId == null) brandId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectCode", projectCode),
                                                        new SqlParameter("@ProjectName", projectName),
                                                        new SqlParameter("@TenantId", tenantId),
                                                        new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@Year", year),
                                                        new SqlParameter("@BrandId", brandId)};
            Type t = typeof(Projects);
            string sql = "";
            sql = @"SELECT * FROM Projects
                    WHERE 1=1 ";
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
            if (!string.IsNullOrEmpty(projectCode))
            {
                sql += " AND ProjectCode = @ProjectCode";
            }
            if (!string.IsNullOrEmpty(projectName))
            {
                sql += " AND ProjectName = @ProjectName";
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

            sql += " ORDER BY InDateTime DESC";
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
                project.ModifyDateTime = DateTime.Now;
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
        /// <param name="projectId"></param>
        /// <param name="key"></param>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<UserInfo> GetUserInfo(string projectId, string key,string shopCode)
        {
            if (projectId == null) projectId = "";
            if (key == null) key = "";
            if (shopCode == null) shopCode = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                       ,new SqlParameter("@Key",key)
                                                        ,new SqlParameter("@ShopCode",shopCode)};
            Type t = typeof(UserInfo);
            string sql = @"SELECT *
                            FROM UserInfo A 
                            WHERE ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(key))
            {
                sql += " AND (ShopCode LIKE '%'+@Key+'%' OR ShopName LIKE '%'+@Key+'%')";
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                sql += " AND ShopCode = @ShopCode";
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
                sql += userInfo.PhotoExpireDateTime.ToString() + "','";
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
        public void ResetPhotoExpireDateTime(string projectId, string expireDateTime)
        {
            string sql = @"UPDATE UserInfo SET  PhotoExpireDateTime = '" + expireDateTime + "' WHERE  ProjectId = '" + projectId + "'";
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
                findOne.PhotoExpireDateTime = userInfo.PhotoExpireDateTime;
                findOne.ModifyDateTime = DateTime.Now;
            }
            db.SaveChanges();
        }
        public void UpdateUserInfoExpireDateTime(UserInfo userInfo)
        {
            string expireDateTime = "";
            if (userInfo.ExpireDateTime != null)
            {
                expireDateTime = Convert.ToDateTime(userInfo.ExpireDateTime).ToString("yyyy-MM-dd") + " 23:59:59";
            }
            string photoExpireDateTime = "";
            if (userInfo.PhotoExpireDateTime != null)
            {
                photoExpireDateTime = Convert.ToDateTime(userInfo.PhotoExpireDateTime).ToString("yyyy-MM-dd") + " 23:59:59";
            }
            string sql = @"UPDATE UserInfo SET  ExpireDateTime = '"  + expireDateTime + "',"
                        + " PhotoExpireDateTime = '"+ photoExpireDateTime + "'"
                        + " WHERE  ProjectId = '" + userInfo.ProjectId.ToString() + "'"
                        +" AND ShopCode = '"+ userInfo.ShopCode+"'";
            SqlParameter[] para = new SqlParameter[] { };
            db.Database.ExecuteSqlCommand(sql, para);
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
                    WHERE 1=1 AND ProjectId = @ProjectId ";
            //if (!string.IsNullOrEmpty(projectId))
            //{
            //    sql += " AND ProjectId = @ProjectId";
            //}
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
                checkType.ModifyDateTime = DateTime.Now;
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
        public List<Remark> GetRemark(string projectId, string checkTypeId, string remarkId, string addCheck, string remarkName, bool? useChk)
        {
            if (projectId == null) projectId = "";
            if (checkTypeId == null) checkTypeId = "";
            if (addCheck == null) addCheck = "";
            if (remarkId == null) remarkId = "";
            if (remarkName == null) remarkName = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                        ,new SqlParameter("@CheckTypeId", checkTypeId)
                                                       , new SqlParameter("@RemarkId", remarkId)
                                                        , new SqlParameter("@AddCheck", addCheck)
                                                        , new SqlParameter("@RemarkName", remarkName)
                                                       };
            Type t = typeof(Remark);
            string sql = "";
            sql = @"SELECT *
                    FROM [Remark]
                    WHERE ProjectId = @ProjectId 
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
            if (remark.AddCheck == "Y") remark.CheckTypeId = null;
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
                findOne.CheckTypeId = remark.CheckTypeId;
                findOne.ProjectId = remark.ProjectId;
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
        public List<PhotoList> GetPhotoList(string projectId, string checkTypeId, string photoId, string addCheck, string photoName, bool? useChk)
        {
            if (projectId == null) projectId = "";
            if (checkTypeId == null) checkTypeId = "";
            if (photoId == null) photoId = "";
            if (addCheck == null) addCheck = "";
            if (photoName == null) photoName = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                    ,new SqlParameter("@CheckTypeId", checkTypeId)
                                                        ,new SqlParameter("@PhotoId", photoId)
                                                        , new SqlParameter("@AddCheck", addCheck)
                                                        , new SqlParameter("@PhotoName", photoName)
                                                    };
            Type t = typeof(PhotoList);
            string sql = "";
            sql = @"SELECT *
                    FROM [PhotoList] 
                    WHERE ProjectId = @ProjectId 
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
            if (photoList.AddCheck == "Y") photoList.CheckTypeId = null;
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
                findOne.CheckTypeId = photoList.CheckTypeId;
                findOne.ProjectId = photoList.ProjectId;
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
                            ,(SELECT ListShowChk FROM ExtendColumnProject WHERE ProjectId =  @ProjectId AND ColumnCode = A.ColumnCode) AS ListShowChk
                            ,(SELECT EditChk FROM ExtendColumnProject WHERE ProjectId =  @ProjectId AND ColumnCode = A.ColumnCode) AS EditChk
                            ,(SELECT ColumnType FROM ExtendColumnProject WHERE ProjectId =  @ProjectId AND ColumnCode = A.ColumnCode) AS ColumnType
                            FROM ExtendColumn A WHERE 1=1";
            if (!string.IsNullOrEmpty(columnCode))
            {
                sql += " AND ColumnCode = @ColumnCode";
            }
            sql += " ORDER BY InDateTime ASC";
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
                findOne.ColumnType = extendColumnProject.ColumnType;
                findOne.AddShowChk = extendColumnProject.AddShowChk;
                findOne.UseChk = extendColumnProject.UseChk;
                findOne.ListShowChk = extendColumnProject.ListShowChk;
                findOne.EditChk = extendColumnProject.EditChk;
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

        public List<FileType> GetFileType()
        {
            SqlParameter[] para = new SqlParameter[] { };
            Type t = typeof(FileType);
            string sql = @"SELECT A.*
                            FROM FileType A ";
            return db.Database.SqlQuery(t, sql, para).Cast<FileType>().ToList();
        }
        public List<FileNameOption> GetFileNameOption(string projectId)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId) };
            Type t = typeof(FileNameOption);
            string sql = @"SELECT OptionCode,OptionName FROM FileNameOption
                            UNION ALL
                        SELECT ColumnCode AS OptionCode,ColumnName AS OptionName FROM dbo.ExtendColumnProject 
                        WHERE ProjectId = @ProjectId";
            return db.Database.SqlQuery(t, sql, para).Cast<FileNameOption>().ToList();
        }
        public void SaveFileRename(FileRename fileRename)
        {
            // 只能新增不能修改
            if (fileRename.SeqNO == 0)
            {
                FileRename findOneMax = db.FileRename.Where(x => (x.ProjectId == fileRename.ProjectId && x.FileTypeCode == fileRename.FileTypeCode)).OrderByDescending(x => x.SeqNO).FirstOrDefault();
                if (findOneMax == null)
                {
                    fileRename.SeqNO = 1;
                }
                else
                {
                    fileRename.SeqNO = findOneMax.SeqNO + 1;
                }
                fileRename.InDateTime = DateTime.Now;
                fileRename.ModifyDateTime = DateTime.Now;
                db.FileRename.Add(fileRename);
            }
            db.SaveChanges();
        }
        public void DeleteFileRename(FileRename fileRename)
        {
            string sql = "DELETE FileRename WHERE FileNameId= '" + fileRename.FileNameId.ToString() + "'";
            SqlParameter[] para = new SqlParameter[] { };
            db.Database.ExecuteSqlCommand(sql, para);
        }
        public List<FileRenameDto> GetFileRename(string projectId, string fileTypeCode)
        {
            if (projectId == null) projectId = "";
            if (fileTypeCode == null) fileTypeCode = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                    ,new SqlParameter("@FileTypeCode", fileTypeCode) };
            Type t = typeof(FileRenameDto);
            string sql = @"SELECT A.*,D.ProjectCode,D.ProjectName,B.FileTypeName
                        ,CASE WHEN EXISTS(SELECT 1 FROM FileNameOption WHERE OptionCode = A.OptionCode) 
                        THEN (SELECT TOP 1 OptionName FROM FileNameOption WHERE OptionCode = A.OptionCode)
                        ELSE (SELECT TOP 1 ColumnName FROM ExtendColumnProject WHERE ColumnCode = A.OptionCode AND ProjectId = 14)
                        END AS OptionName
                         FROM FileRename A INNER JOIN FileType B ON A.FileTypeCode = B.FileTypeCode
				                       -- LEFT JOIN FileNameOption C ON A.OptionCode = C.OptionCode
				                    INNER JOIN Projects D ON A.ProjectId = D.ProjectId
                        WHERE A.ProjectId = @ProjectId";
            if (!string.IsNullOrEmpty(fileTypeCode))
            {
                sql += " AND A.FileTypeCode = @FileTypeCode";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<FileRenameDto>().ToList();
        }
    }
}