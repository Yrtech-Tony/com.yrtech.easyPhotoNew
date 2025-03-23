using com.yrtech.InventoryAPI.Common;
using com.yrtech.InventoryAPI.DTO;
using com.yrtech.InventoryDAL;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace com.yrtech.InventoryAPI.Service
{
    public class AnswerService
    {
        com.yrtech.InventoryDAL.InventoryDAL db = new InventoryDAL.InventoryDAL();
        MasterService masterService = new MasterService();
        AccountService accountService = new AccountService();
        #region 盘点任务
        public List<TaskDto> GetTask(string tenantId, string brandId, string projectId, string taskId, string taskCode, string status, string key, DateTime? startDate, DateTime? endDate)
        {

            if (projectId == null) projectId = "";
            if (tenantId == null) tenantId = "";
            if (brandId == null) brandId = "";
            if (taskCode == null) taskCode = "";
            if (taskId == null) taskId = "";
            if (key == null) key = "";
            if (status == null) status = "";
            if (startDate == null) startDate = new DateTime(2000, 1, 1);
            if (endDate == null) endDate = new DateTime(3000, 1, 1);


            SqlParameter[] para = new SqlParameter[] {
                                                        new SqlParameter("@TenantId", tenantId),
                                                        new SqlParameter("@BrandId", brandId),
                                                        new SqlParameter("@ProjectId", projectId),
                                                        new SqlParameter("@Key", key),
                                                        new SqlParameter("@TaskId", taskId),
                                                        new SqlParameter("@TaskCode", taskCode),
                                                        new SqlParameter("@StartDate", startDate),
                                                        new SqlParameter("@EndDate", endDate)
            };
            Type t = typeof(TaskDto);
            string sql = "";
            sql = @"SELECT A.*
                    ,CASE WHEN EXISTS(SELECT 1 FROM TaskStatus X WHERE X.TaskId = A.TaskId AND X.StatusCode = 'S2')
                    THEN '已审核'
                    WHEN EXISTS(SELECT 1 FROM TaskStatus X WHERE X.TaskId = A.TaskId AND X.StatusCode = 'S1')
                    THEN '已提交'
                    WHEN NOT EXISTS(SELECT 1 FROM TaskStatus X WHERE X.TaskId = A.TaskId AND X.StatusCode = 'S1') 
                         AND GETDATE()>ExpireDateTime
                    THEN '超时'
                    ELSE '未提交'
                    END AS StatusName
                    ,ISNULL((SELECT COUNT(*) FROM Answer WHERE TaskId = A.TaskId),0) AS AnswerCount
                    FROM Task A INNER JOIN Projects B ON A.ProjectId = B.ProjectId
                    WHERE 1=1 AND A.StartDate BETWEEN @StartDate AND @EndDate";
            if (!string.IsNullOrEmpty(tenantId))
            {
                sql += " AND B.TenantId = @TenantId";
            }
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND B.ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(brandId))
            {
                sql += " AND B.BrandId = @BrandId";
            }
            if (!string.IsNullOrEmpty(taskId))
            {
                sql += " AND A.TaskId = @TaskId";
            }
            if (!string.IsNullOrEmpty(taskCode))
            {
                sql += " AND A.TaskCode = @TaskCode";
            }
            // 关键字按照任务和仓库的代码模糊查询
            if (!string.IsNullOrEmpty(key))
            {
                sql += @" AND (A.TaskCode LIKE '%'+ @Key+'%' 
                               OR A.TaskName LIKE '%'+@Key+'%' 
                               OR EXISTS(SELECT 1 
                                    FROM Answer X WHERE X.TaskId = A.TaskId
                                                         AND(X.ShopCode LIKE '%' + @Key + '%' OR X.ShopName LIKE '%' + @Key + '%')
                                                        ))";
            }
            if (!string.IsNullOrEmpty(status)) // 
            {
                if (status == "0") // 未提交
                {
                    sql += " AND NOT EXISTS(SELECT 1 FROM TaskStatus U WHERE U.TaskId = A.TaskId)";
                }
                else if (status == "1") // 已提交
                {
                    sql += " AND  EXISTS(SELECT 1 FROM TaskStatus U WHERE U.TaskId = A.TaskId AND U.StatusCode = 'S1')";
                }
                else if (status == "2") // 已审核
                {
                    sql += " AND  EXISTS(SELECT 1 FROM TaskStatus U WHERE U.TaskId = A.TaskId AND U.StatusCode = 'S2')";
                }
            }
            List<TaskDto> list = db.Database.SqlQuery(t, sql, para).Cast<TaskDto>().ToList();
            list = list.OrderByDescending(x => x.StartDate).ToList();
            return list;

        }
        public List<TaskDto> GetTaskForMobile(string projectId, string shopCode, string status)
        {

            if (projectId == null) projectId = "";
            if (status == null) status = "";
            if (shopCode == null) shopCode = "";
            SqlParameter[] para = new SqlParameter[] {
                                                       new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@Status", status),
                                                       new SqlParameter("@ShopCode", shopCode)
            };
            Type t = typeof(TaskDto);
            string sql = "";
            sql = @"SELECT A.*
                    ,CASE WHEN EXISTS(SELECT 1 FROM TaskStatus X WHERE X.TaskId = A.TaskId AND X.StatusCode = 'S2')
                    THEN '已审核'
                    WHEN EXISTS(SELECT 1 FROM TaskStatus X WHERE X.TaskId = A.TaskId AND X.StatusCode = 'S1')
                    THEN '已提交'
                    WHEN NOT EXISTS(SELECT 1 FROM TaskStatus X WHERE X.TaskId = A.TaskId AND X.StatusCode = 'S1') 
                         AND GETDATE()>ExpireDateTime
                    THEN '超时'
                    ELSE '未提交'
                    END AS StatusName
                    ,ISNULL((SELECT COUNT(*) FROM Answer WHERE TaskId = A.TaskId),0) AS AnswerCount
                    FROM Task A INNER JOIN Projects B ON A.ProjectId = B.ProjectId
                    WHERE 1=1 AND A.ExpireDateTime>GETDATE()";

            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND B.ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                sql += " AND EXISTS(SELECT 1 FROM Answer X WHERE X.TaskId = A.TaskId AND X.ShopCode = @ShopCode)";
            }
            if (!string.IsNullOrEmpty(status)) // 
            {
                if (status == "0") // 未提交
                {
                    sql += " AND NOT EXISTS(SELECT 1 FROM TaskStatus U WHERE U.TaskId = A.TaskId)";
                }
                else if (status == "1") // 已提交
                {
                    sql += " AND  EXISTS(SELECT 1 FROM TaskStatus U WHERE U.TaskId = A.TaskId AND U.StatusCode = 'S1')";
                }
                else if (status == "2") // 已审核
                {
                    sql += " AND  EXISTS(SELECT 1 FROM TaskStatus U WHERE U.TaskId = A.TaskId AND U.StatusCode = 'S2')";
                }
            }
            List<TaskDto> list = db.Database.SqlQuery(t, sql, para).Cast<TaskDto>().ToList();
            list = list.OrderByDescending(x => x.StartDate).ToList();
            return list;

        }
        public Task SaveTask(Task task)
        {
            Task findOne = db.Task.Where(x => (x.TaskId == task.TaskId)).FirstOrDefault();
            if (findOne == null)
            {
                task.InDateTime = DateTime.Now;
                task.ModifyDateTime = DateTime.Now;
                db.Task.Add(task);
                db.SaveChanges();
                return task;
            }
            else
            {
                findOne.ProjectId = task.ProjectId;
                findOne.TaskCode = task.TaskCode;
                findOne.TaskName = task.TaskName;
                findOne.StartDate = task.StartDate;
                findOne.ExpireDateTime = task.ExpireDateTime;
                findOne.ModifyUserId = task.ModifyUserId;
                findOne.ModifyDateTime = DateTime.Now;
                task = findOne;
                db.SaveChanges();
                return task;
            }

        }
        #endregion
        #region 清单管理

        public List<AnswerDto> GetShopAnswerListAll(string answerId, string taskId, string shopCode, string checkCode, string checkTypeId, string photoCheck, string addCheck, string key)
        {
            if (taskId == null) taskId = "";
            if (shopCode == null) shopCode = "";
            if (checkCode == null) checkCode = "";
            if (checkTypeId == null) checkTypeId = "";
            if (photoCheck == null) photoCheck = "";
            if (addCheck == null) addCheck = "";
            if (answerId == null) answerId = "";
            if (key == null) key = "";
            SqlParameter[] para = new SqlParameter[] {  new SqlParameter("@AnswerId", answerId),
                                                        new SqlParameter("@TaskId", taskId),
                                                        new SqlParameter("@ShopCode", shopCode),
                                                        new SqlParameter("@CheckCode", checkCode),
                                                        new SqlParameter("@CheckTypeId", checkTypeId),
                                                        new SqlParameter("@PhotoCheck", photoCheck),
                                                        new SqlParameter("@AddCheck", addCheck),
                                                        new SqlParameter("@Key", key)};
            Type t = typeof(AnswerDto);
            string sql = "";
            sql = @"SELECT A.AnswerId,A.TaskId,A.ShopCode,A.ShopName,A.CheckCode,RIGHT(A.CheckCode,8) AS CheckCodeShow
                    ,A.CheckTypeId
                    ,ISNULL((SELECT CheckTypeName FROM CheckType WHERE CheckTypeId = A.CheckTypeId AND ProjectId = B.ProjectId),'') AS CheckTypeName
                    ,ISNULL((SELECT TOP 1 RecheckStatus FROM Recheck WHERE AnswerId = A.AnswerId),0) AS RecheckStatus
                    ,A.Remark AS RemarkName,A.AddCheck,A.ModifyUserId,A.ModifyDateTime,A.InUserId,A.InDateTime,
                    CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = B.ProjectId AND ColumnCode = 'Column1' AND UseChk = 1)
                         THEN ISNULL(Column1,'')
                    END AS Column1,
                    CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = B.ProjectId AND ColumnCode = 'Column2' AND UseChk = 1)
                         THEN ISNULL(Column2,'')
                    END AS Column2,
                    CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = B.ProjectId AND ColumnCode = 'Column3' AND UseChk = 1)
                         THEN ISNULL(Column3,'')
                    END AS Column3,
                    CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = B.ProjectId AND ColumnCode = 'Column4' AND UseChk = 1)
                         THEN ISNULL(Column4,'')
                    END AS Column4,
                    CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = B.ProjectId AND ColumnCode = 'Column5' AND UseChk = 1)
                         THEN ISNULL(Column5,'')
                    END AS Column5,
                    CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = B.ProjectId AND ColumnCode = 'Column6' AND UseChk = 1)
                         THEN ISNULL(Column6,'')
                    END AS Column6,
                    CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = B.ProjectId AND ColumnCode = 'Column7' AND UseChk = 1)
                         THEN ISNULL(Column7,'')
                    END AS Column7,
                    CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = B.ProjectId AND ColumnCode = 'Column8' AND UseChk = 1)
                         THEN ISNULL(Column8,'')
                    END AS Column8,
                    CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = B.ProjectId AND ColumnCode = 'Column9' AND UseChk = 1)
                         THEN ISNULL(Column9,'')
                    END AS Column9,
                     CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = B.ProjectId AND ColumnCode = 'Column10' AND UseChk = 1)
                         THEN ISNULL(Column10,'')
                    END AS Column10,
                     CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = B.ProjectId AND ColumnCode = 'Column11' AND UseChk = 1)
                         THEN ISNULL(Column11,'')
                    END AS Column11,
                     CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = B.ProjectId AND ColumnCode = 'Column12' AND UseChk = 1)
                         THEN ISNULL(Column12,'')
                    END AS Column12,
                     CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = B.ProjectId AND ColumnCode = 'Column13' AND UseChk = 1)
                         THEN ISNULL(Column13,'')
                    END AS Column13,
                     CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = B.ProjectId AND ColumnCode = 'Column14' AND UseChk = 1)
                         THEN ISNULL(Column14,'')
                    END AS Column14,
                     CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = B.ProjectId AND ColumnCode = 'Column15' AND UseChk = 1)
                         THEN ISNULL(Column15,'')
                    END AS Column15
                    ,CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = B.ProjectId AND ColumnCode = 'Column16' AND UseChk = 1)
                         THEN ISNULL(Column16,'')
                    END AS Column16
                    ,CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = B.ProjectId AND ColumnCode = 'Column17' AND UseChk = 1)
                         THEN ISNULL(Column17,'')
                    END AS Column17
                    ,CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = B.ProjectId AND ColumnCode = 'Column18' AND UseChk = 1)
                         THEN ISNULL(Column18,'')
                    END AS Column18
                    ,CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = B.ProjectId AND ColumnCode = 'Column19' AND UseChk = 1)
                         THEN ISNULL(Column19,'')
                    END AS Column19
                    ,CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = B.ProjectId AND ColumnCode = 'Column20' AND UseChk = 1)
                         THEN ISNULL(Column20,'')
                    END AS Column20
                    FROM Answer A INNER JOIN Task B ON A.TaskId=B.TaskId
                    WHERE 1=1 ";
            if (!string.IsNullOrEmpty(answerId))
            {
                sql += " AND A.AnswerId = @AnswerId";
            }
            if (!string.IsNullOrEmpty(taskId))
            {
                sql += " AND B.TaskId = @TaskId";
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                sql += " AND A.ShopCode = @ShopCode";
            }
            if (!string.IsNullOrEmpty(checkCode))
            {
                sql += " AND A.CheckCode LIKE '%'+@CheckCode+'%'";
            }
            if (!string.IsNullOrEmpty(checkTypeId))
            {
                sql += " AND A.CheckTypeId = @CheckTypeId";
            }
            if (!string.IsNullOrEmpty(photoCheck))
            {
                if (photoCheck == "Y")
                {
                    sql += @" AND EXISTS(SELECT 1 FROM AnswerPhoto WHERE AnswerId = A.AnswerId )";
                }
                else
                {
                    sql += @" AND NOT EXISTS (SELECT 1 FROM AnswerPhoto WHERE AnswerId =A.AnswerId ) ";
                }
            }
            if (!string.IsNullOrEmpty(addCheck))
            {
                sql += " AND A.AddCheck = @AddCheck";
            }
            if (!string.IsNullOrEmpty(key))
            {
                key = key.Replace("，", ",");
                sql += " AND ShopCode IN('";
                string[] shopList = key.Split(',');
                foreach (string shop in shopList)
                {
                    if (shop == shopList[shopList.Length - 1])
                    {
                        sql += shop + "'";
                    }
                    else
                    {
                        sql += shop + "','";
                    }
                }
                sql += ")";
            }
            sql += " Order BY B.ProjectId,A.ShopCode,A.CheckTypeId, CheckCode";
            List<AnswerDto> list = db.Database.SqlQuery(t, sql, para).Cast<AnswerDto>().ToList();
            // 子查询太多，性能比较慢
            list = list.OrderBy(x => x.ShopCode).ThenBy(x => x.CheckTypeId).ThenBy(x => x.CheckCode).ToList();
            return list;
        }
        public List<AnswerDto> GetShopAnswerListByPage(string answerId, string projectId, string shopCode, string checkCode, string checkTypeId, string photoCheck, string addCheck, string key, int pageNum, int pageCount)
        {
            int startIndex = (pageNum - 1) * pageCount;

            return GetShopAnswerListAll(answerId, projectId, shopCode, checkCode, checkTypeId, photoCheck, addCheck, key).Skip(startIndex).Take(pageCount).ToList();
        }
        public List<AnswerPhotoDto> GetAnswerPhotoList(string answerId, string taskId, string shopCode)
        {
            if (answerId == null) answerId = "";
            if (taskId == null) taskId = "";
            if (shopCode == null) shopCode = "";

            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@AnswerId", answerId)
                                                    ,new SqlParameter("@TaskId", taskId)
                                                    ,new SqlParameter("@ShopCode", shopCode)
                                                      };
            Type t = typeof(AnswerPhotoDto);
            string sql = "";
            sql = @"SELECT A.AnswerId,A.TaskId,A.CheckCode,B.PhotoId,B.PhotoUrl,C.PhotoName,A.ShopCode,A.ShopName,ISNULL(C.MustChk,0) AS MustChk
                    ,ISNULL(D.CheckTypeName,'') AS CheckTypeName
                    FROM Answer A INNER JOIN AnswerPhoto B ON A.AnswerId = B.AnswerId
                                  INNER JOIN Task X ON A.TaskId = X.TaskId
                                  INNER JOIN PhotoList C ON B.PhotoId = C.PhotoId 
                                  LEFT JOIN CheckType D ON A.CheckTypeId = D.CheckTypeId AND X.ProjectId = D.ProjectId
                    WHERE 1=1 ";
            if (!string.IsNullOrEmpty(taskId))
            {
                sql += " AND A.TaskId = @TaskId";
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                if (!string.IsNullOrEmpty(shopCode))
                {
                    shopCode = shopCode.Replace("，", ",");
                    sql += " AND A.ShopCode IN('";
                    string[] shopList = shopCode.Split(',');
                    foreach (string shop in shopList)
                    {
                        if (shop == shopList[shopList.Length - 1])
                        {
                            sql += shop + "'";
                        }
                        else
                        {
                            sql += shop + "','";
                        }
                    }
                    sql += ")";
                }
                //sql += " AND A.ShopCode = @ShopCode";
            }
            if (!string.IsNullOrEmpty(answerId))
            {
                sql += " AND A.AnswerId = @AnswerId";
            }
            sql += " Order By B.PhotoId";
            return db.Database.SqlQuery(t, sql, para).Cast<AnswerPhotoDto>().ToList();
        }
        public List<AnswerPhotoDto> GetAnswerPhotoExport(string answerId, string projectId, string shopCode)
        {
            if (answerId == null) answerId = "";
            if (projectId == null) projectId = "";
            if (shopCode == null) shopCode = "";

            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@AnswerId", answerId)
                                                    ,new SqlParameter("@ProjectId", projectId)
                                                    ,new SqlParameter("@ShopCode", shopCode)
                                                      };
            Type t = typeof(AnswerPhotoDto);
            string sql = "";
            sql = @"SELECT * FROM 
                            (SELECT 
                            A.ProjectId,A.ShopCode,A.ShopName,A.CheckCode,C.CheckTypeName,'N' AS AddCheck,B.PhotoName
                            ,CASE WHEN EXISTS(SELECT 1 FROM dbo.AnswerPhoto WHERE AnswerId = A.AnswerId AND PhotoId = B.PhotoId)
	                            THEN 'Y'
	                            ELSE 'N'
                            END AS Photo
                            from answer A INNER JOIN PhotoList B ON A.ProjectId = B.ProjectId 
									                            AND  A.CheckTypeId = B.CheckTypeId 
									                            AND B.AddCheck = 'N'
			                              INNER JOIN CheckType C ON A.CheckTypeId = C.CheckTypeId

                            UNION ALL

                            SELECT 
                            A.ProjectId,A.ShopCode,A.ShopName,A.CheckCode,'' AS CheckTypeName,'Y' AS AddCheck,B.PhotoName
                            ,CASE WHEN EXISTS(SELECT 1 FROM dbo.AnswerPhoto WHERE AnswerId = A.AnswerId AND PhotoId = B.PhotoId)
	                            THEN 'Y'
	                            ELSE 'N'
                            END AS Photo
                            from answer A INNER JOIN PhotoList B ON A.ProjectId = B.ProjectId  
									                            AND B.AddCheck = 'Y' ) X WHERE 1=1 
			   ";
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND X.ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                if (!string.IsNullOrEmpty(shopCode))
                {
                    shopCode = shopCode.Replace("，", ",");
                    sql += " AND X.ShopCode IN('";
                    string[] shopList = shopCode.Split(',');
                    foreach (string shop in shopList)
                    {
                        if (shop == shopList[shopList.Length - 1])
                        {
                            sql += shop + "'";
                        }
                        else
                        {
                            sql += shop + "','";
                        }
                    }
                    sql += ")";
                }
            }
            sql += " Order By X.ShopCode,X.ShopName,X.CheckCode";
            return db.Database.SqlQuery(t, sql, para).Cast<AnswerPhotoDto>().ToList();
        }
        public void ImportAnswerList(string projectId, List<AnswerDto> answerList)
        {
            string sql = "";
            SqlParameter[] para = new SqlParameter[] { };
            //sql += "DELETE AnswerPhoto WHERE AnswerId IN (SELECT AnswerId FROM Answer WHERE ProjectId='" + projectId + "')";
            //sql += " DELETE Answer WHERE ProjectId='" + projectId + "'";
            foreach (AnswerDto answer in answerList)
            {
                int checkTypeId = 0;
                checkTypeId = masterService.GetCheckType(projectId, "", answer.CheckTypeName, true)[0].CheckTypeId;
                sql += " INSERT INTO Answer VALUES('";
                sql += projectId + "','";
                sql += answer.ShopCode.ToString() + "','";
                sql += answer.ShopName.ToString() + "','";
                sql += answer.CheckCode + "','";
                sql += answer.CheckTypeId.ToString() + "','";
                sql += "','";
                sql += answer.Column1 + "','";
                sql += answer.Column2 + "','";
                sql += answer.Column3 + "','";
                sql += answer.Column4 + "','";
                sql += answer.Column5 + "','";
                sql += answer.Column6 + "','";
                sql += answer.Column7 + "','";
                sql += answer.Column8 + "','";
                sql += answer.Column9 + "','";
                sql += answer.Column10 + "','";
                sql += answer.Column11 + "','";
                sql += answer.Column12 + "','";
                sql += answer.Column13 + "','";
                sql += answer.Column14 + "','";
                sql += answer.Column15 + "','";
                sql += answer.Column16 + "','";
                sql += answer.Column17 + "','";
                sql += answer.Column18 + "','";
                sql += answer.Column19 + "','";
                sql += answer.Column20 + "','";
                sql += "N" + "',";
                sql += answer.InUserID + ",GETDATE(),";
                sql += answer.ModifyUserId + ",GETDATE())";
                sql += " ";
            }
            db.Database.ExecuteSqlCommand(sql, para);
        }
        public Answer SaveShopAnswer(Answer answer,string openId)
        {
            Answer findOne = db.Answer.Where(x => (x.AnswerId == answer.AnswerId)).FirstOrDefault();
            if (findOne == null)
            {
                answer.InDateTime = DateTime.Now;
                answer.ModifyDateTime = DateTime.Now;
                answer.AddCheck = "Y";
                answer = db.Answer.Add(answer);
                db.SaveChanges();
                SaveShopAnswerLog(answer, openId);
            }
            else
            {
                findOne.CheckCode = answer.CheckCode;
                findOne.Remark = answer.Remark;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = answer.ModifyUserId;
                findOne.Column1 = answer.Column1;
                findOne.Column2 = answer.Column2;
                findOne.Column3 = answer.Column3;
                findOne.Column4 = answer.Column4;
                findOne.Column5 = answer.Column5;
                findOne.Column6 = answer.Column6;
                findOne.Column7 = answer.Column7;
                findOne.Column8 = answer.Column8;
                findOne.Column9 = answer.Column9;
                findOne.Column10 = answer.Column10;
                findOne.Column11 = answer.Column11;
                findOne.Column12 = answer.Column12;
                findOne.Column13 = answer.Column13;
                findOne.Column14 = answer.Column14;
                findOne.Column15 = answer.Column15;
                findOne.Column16 = answer.Column16;
                findOne.Column17 = answer.Column17;
                findOne.Column18 = answer.Column18;
                findOne.Column19 = answer.Column19;
                findOne.Column20 = answer.Column20;
                answer = findOne;
                db.SaveChanges();
                SaveShopAnswerLog(findOne, openId);
            }
            
            return answer;
        }
        public void SaveShopAnswerLog(Answer answer,string openId)
        {
            AnswerLog answerLog = new AnswerLog();
            answerLog.AnswerId = answer.AnswerId;
            answerLog.TaskId = answer.TaskId;
            answerLog.ShopCode = answer.ShopCode;
            answerLog.ShopName = answer.ShopName;
            answerLog.CheckTypeId = answer.CheckTypeId;
            answerLog.AddCheck = answer.AddCheck;
            answerLog.CheckCode = answer.CheckCode;
            answerLog.Remark = answer.Remark;
            answerLog.InUserID = answer.InUserID;
            answerLog.InDateTime = DateTime.Now;
            answerLog.ModifyDateTime = DateTime.Now;
            answerLog.ModifyUserId = answer.ModifyUserId;
            answerLog.OpenId = openId;
            answerLog.Column1 = answer.Column1;
            answerLog.Column2 = answer.Column2;
            answerLog.Column3 = answer.Column3;
            answerLog.Column4 = answer.Column4;
            answerLog.Column5 = answer.Column5;
            answerLog.Column6 = answer.Column6;
            answerLog.Column7 = answer.Column7;
            answerLog.Column8 = answer.Column8;
            answerLog.Column9 = answer.Column9;
            answerLog.Column10 = answer.Column10;
            answerLog.Column11 = answer.Column11;
            answerLog.Column12 = answer.Column12;
            answerLog.Column13 = answer.Column13;
            answerLog.Column14 = answer.Column14;
            answerLog.Column15 = answer.Column15;
            answerLog.Column16 = answer.Column16;
            answerLog.Column17 = answer.Column17;
            answerLog.Column18 = answer.Column18;
            answerLog.Column19 = answer.Column19;
            answerLog.Column20 = answer.Column20;
            db.AnswerLog.Add(answerLog);
            db.SaveChanges();
           
        }
        public void DeleteShopAnswer(string[] answerList)
        {
            string sql = "";
            SqlParameter[] para = new SqlParameter[] { };
            foreach (string answerId in answerList)
            {
                sql += "DELETE AnswerPhoto WHERE AnswerId = " + answerId + " ";
                sql += "DELETE Answer WHERE AnswerId = " + answerId + " ";
            }
            db.Database.ExecuteSqlCommand(sql, para);
        }
        public void DeleteAnswerByShop(string projectId, string shopCode)
        {
            string[] shopList = shopCode.Split(',');
            string sql = "";
            SqlParameter[] para = new SqlParameter[] { };
            foreach (string shop in shopList)
            {
                sql += "DELETE AnswerPhoto WHERE AnswerId IN(SELECT AnswerId FROM Answer WHERE ProjectId= " + projectId + " AND ShopCode = '" + shop + "') ";
                sql += "DELETE Answer WHERE ProjectId = " + projectId + " AND ShopCode = '" + shop + "'";
            }
            db.Database.ExecuteSqlCommand(sql, para);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="answerPhoto"></param>
        public void SaveShopAnswerPhoto(AnswerPhoto answerPhoto,string openId)
        {
            if (answerPhoto.PhotoId == 0) return;
            AnswerPhoto findOne = db.AnswerPhoto.Where(x => (x.AnswerId == answerPhoto.AnswerId && x.PhotoId == answerPhoto.PhotoId)).FirstOrDefault();
            if (findOne == null)
            {
                answerPhoto.InDateTime = DateTime.Now;
                answerPhoto.ModifyDateTime = DateTime.Now;
                db.AnswerPhoto.Add(answerPhoto);
                db.SaveChanges();
                SaveShopAnswerPhotoLog(answerPhoto, openId);
            }
            else
            {
                findOne.PhotoUrl = answerPhoto.PhotoUrl;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = answerPhoto.ModifyUserId;
                db.SaveChanges();
                SaveShopAnswerPhotoLog(findOne, openId);
            }
           
        }
        public void SaveShopAnswerPhotoLog(AnswerPhoto answerPhoto,string openId)
        {
            AnswerPhotoLog answerPhotoLog = new AnswerPhotoLog();
            answerPhotoLog.AnswerId = answerPhoto.AnswerId;
            answerPhotoLog.InDateTime = DateTime.Now;
            answerPhotoLog.InUserId = answerPhoto.InUserId;
            answerPhotoLog.OpenId = openId;
            answerPhotoLog.PhotoId = answerPhoto.PhotoId;
            answerPhotoLog.PhotoUrl = answerPhoto.PhotoUrl;
            answerPhotoLog.ModifyDateTime = DateTime.Now;
            answerPhotoLog.ModifyUserId = answerPhoto.ModifyUserId;
            db.AnswerPhotoLog.Add(answerPhotoLog);
            db.SaveChanges();
        }
        public void DeleteShopAnswerPhoto(string answerId, string photoId)
        {
            if (photoId == null) photoId = "";
            string sql = "";
            SqlParameter[] para = new SqlParameter[] { };
            sql += "DELETE AnswerPhoto WHERE AnswerId = " + answerId.ToString() + " ";
            if (!string.IsNullOrEmpty(photoId))
            {
                sql += " AND PhotoId = " + photoId.ToString() + " ";
            }
            db.Database.ExecuteSqlCommand(sql, para);
        }
        #endregion
        #region 照片下载
        /// <summary>
        /// 下载照片时使用，把扩展列拼成List形式
        /// </summary>
        /// <param name="answerId"></param>
        /// <returns></returns>
        public List<ExtendColumnProjectDto> GetAnswerColumnListByAnswerId(string answerId, string projectId)
        {
            List<ExtendColumnProjectDto> columnList = new List<ExtendColumnProjectDto>();
            List<AnswerDto> answerList = GetShopAnswerListAll(answerId, projectId, "", "", "", "", "", "");
            if (answerList != null && answerList.Count > 0)
            {
                AnswerDto ansewr = answerList[0];
                columnList = masterService.GetExtendColumnProject(projectId, "");
                foreach (ExtendColumnProjectDto column in columnList)
                {
                    if (column.ColumnCode == "Column1")
                    {
                        column.ColumnAnswer = ansewr.Column1;
                    }
                    if (column.ColumnCode == "Column2")
                    {
                        column.ColumnAnswer = ansewr.Column2;
                    }
                    if (column.ColumnCode == "Column3")
                    {
                        column.ColumnAnswer = ansewr.Column3;
                    }
                    if (column.ColumnCode == "Column4")
                    {
                        column.ColumnAnswer = ansewr.Column4;
                    }
                    if (column.ColumnCode == "Column5")
                    {
                        column.ColumnAnswer = ansewr.Column5;
                    }
                    if (column.ColumnCode == "Column6")
                    {
                        column.ColumnAnswer = ansewr.Column6;
                    }
                    if (column.ColumnCode == "Column7")
                    {
                        column.ColumnAnswer = ansewr.Column7;
                    }
                    if (column.ColumnCode == "Column8")
                    {
                        column.ColumnAnswer = ansewr.Column8;
                    }
                    if (column.ColumnCode == "Column9")
                    {
                        column.ColumnAnswer = ansewr.Column9;
                    }
                    if (column.ColumnCode == "Column10")
                    {
                        column.ColumnAnswer = ansewr.Column10;
                    }
                    if (column.ColumnCode == "Column11")
                    {
                        column.ColumnAnswer = ansewr.Column11;
                    }
                    if (column.ColumnCode == "Column12")
                    {
                        column.ColumnAnswer = ansewr.Column12;
                    }
                    if (column.ColumnCode == "Column13")
                    {
                        column.ColumnAnswer = ansewr.Column13;
                    }
                    if (column.ColumnCode == "Column14")
                    {
                        column.ColumnAnswer = ansewr.Column14;
                    }
                    if (column.ColumnCode == "Column15")
                    {
                        column.ColumnAnswer = ansewr.Column15;
                    }
                    if (column.ColumnCode == "Column16")
                    {
                        column.ColumnAnswer = ansewr.Column16;
                    }
                    if (column.ColumnCode == "Column17")
                    {
                        column.ColumnAnswer = ansewr.Column17;
                    }
                    if (column.ColumnCode == "Column18")
                    {
                        column.ColumnAnswer = ansewr.Column18;
                    }
                    if (column.ColumnCode == "Column19")
                    {
                        column.ColumnAnswer = ansewr.Column19;
                    }
                    if (column.ColumnCode == "Column20")
                    {
                        column.ColumnAnswer = ansewr.Column20;
                    }
                }
            }
            return columnList;
        }
        public string GetFolderName(string projectId, string fileTypeCode, string shopCode, string shopName, string checkCode, string photoName, string checkType, List<ExtendColumnProjectDto> columneList)
        {
            string folderName = "";
            List<FileRenameDto> fileRenameList_Folder = masterService.GetFileRename(projectId, fileTypeCode);
            for (int i = 0; i < fileRenameList_Folder.Count; i++)
            {
                if (fileRenameList_Folder[i].SeqNO == i + 1)
                {
                    if (fileRenameList_Folder[i].OptionCode == "ProjectCode")
                    {
                        folderName += fileRenameList_Folder[i].ProjectCode + fileRenameList_Folder[i].ConnectStr;
                    }

                    if (fileRenameList_Folder[i].OptionCode == "ProjectName")
                    {
                        folderName += fileRenameList_Folder[i].ProjectName + fileRenameList_Folder[i].ConnectStr;
                    }
                    if (fileRenameList_Folder[i].OptionCode == "ShopCode")
                    {
                        folderName += shopCode + fileRenameList_Folder[i].ConnectStr;
                    }
                    if (fileRenameList_Folder[i].OptionCode == "ShopName")
                    {
                        folderName += shopName + fileRenameList_Folder[i].ConnectStr;
                    }
                    if (fileRenameList_Folder[i].OptionCode == "Other")
                    {
                        folderName += fileRenameList_Folder[i].OtherName + fileRenameList_Folder[i].ConnectStr;
                    }
                    if (fileRenameList_Folder[i].OptionCode == "CheckCode")
                    {
                        if (Convert.ToInt32(fileRenameList_Folder[i].EndIndex) > checkCode.Length || fileRenameList_Folder[i].EndIndex == null)
                        {
                            fileRenameList_Folder[i].EndIndex = checkCode.Length;// 如果设置的结束位置大于总长度，取全部
                        }
                        if (Convert.ToInt32(fileRenameList_Folder[i].StartIndex) > checkCode.Length || fileRenameList_Folder[i].StartIndex == null)
                        {
                            fileRenameList_Folder[i].StartIndex = 1;// 若设置的开始位置大于总长度，从0开始
                        }

                        checkCode = checkCode.Substring(Convert.ToInt32(fileRenameList_Folder[i].StartIndex) - 1, Convert.ToInt32(fileRenameList_Folder[i].EndIndex) - Convert.ToInt32(fileRenameList_Folder[i].StartIndex) + 1);
                        folderName += checkCode + fileRenameList_Folder[i].ConnectStr;
                    }
                    if (fileRenameList_Folder[i].OptionCode == "CheckType")
                    {
                        folderName += checkType + fileRenameList_Folder[i].ConnectStr;
                    }
                    if (fileRenameList_Folder[i].OptionCode == "PhotoName")
                    {
                        folderName += photoName + fileRenameList_Folder[i].ConnectStr;
                    }
                    foreach (ExtendColumnProjectDto column in columneList)
                    {
                        if (fileRenameList_Folder[i].OptionCode == column.ColumnCode)
                        {
                            if (!string.IsNullOrEmpty(column.ColumnAnswer))
                            {
                                folderName += column.ColumnAnswer + fileRenameList_Folder[i].ConnectStr;
                            }
                            else
                            {
                                folderName += "无数据" + fileRenameList_Folder[i].ConnectStr;
                            }
                        }
                    }
                }
            }
            return folderName;
        }
        public string AnswerPhotoDownLoad(string projectId, string shopCode)
        {

            List<AnswerPhotoDto> list = GetAnswerPhotoList("", projectId, shopCode);
            if (list == null || list.Count == 0) return "";
            string defaultPath = HostingEnvironment.MapPath(@"~/");
            string basePath = defaultPath + "DownLoadFile";//根目录
            string downLoadfolder = DateTime.Now.ToString("yyyyMMddHHmmssfff");//文件下载的文件夹
            string folder = basePath + @"\" + downLoadfolder;// 文件下载的路径
            string downLoadPath = basePath + @"\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".zip";//打包后的文件名
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            // 从OSS把文件下载到服务器
            foreach (AnswerPhotoDto photo in list)
            {
                string folder1 = GetFolderName(photo.ProjectId.ToString(), "1", photo.ShopCode, photo.ShopName, "", "", photo.CheckTypeName, GetAnswerColumnListByAnswerId(photo.AnswerId.ToString(), photo.ProjectId.ToString()));
                string folder2 = GetFolderName(photo.ProjectId.ToString(), "2", photo.ShopCode, photo.ShopName, "", "", photo.CheckTypeName, GetAnswerColumnListByAnswerId(photo.AnswerId.ToString(), photo.ProjectId.ToString()));
                string folder3 = GetFolderName(photo.ProjectId.ToString(), "3", photo.ShopCode, photo.ShopName, "", "", photo.CheckTypeName, GetAnswerColumnListByAnswerId(photo.AnswerId.ToString(), photo.ProjectId.ToString()));
                string folder4 = GetFolderName(photo.ProjectId.ToString(), "4", photo.ShopCode, photo.ShopName, photo.CheckCode, photo.PhotoName, photo.CheckTypeName, GetAnswerColumnListByAnswerId(photo.AnswerId.ToString(), photo.ProjectId.ToString()));
                if (!string.IsNullOrEmpty(folder4)) // 照片的命名方式是否设置，如果设置了按照设置的方式下载
                { // 创建1级目录
                    if (!string.IsNullOrEmpty(folder1))
                    {
                        if (!Directory.Exists(folder + @"\" + folder1))
                        {
                            Directory.CreateDirectory(folder + @"\" + folder1);
                        }
                    }  // 创建2级目录
                    if (!string.IsNullOrEmpty(folder2))
                    {
                        if (!Directory.Exists(folder + @"\" + folder1 + @"\" + folder2))
                        {
                            Directory.CreateDirectory(folder + @"\" + folder1 + @"\" + folder2);
                        }
                    }
                    // 创建3级目录
                    if (!string.IsNullOrEmpty(folder3))
                    {
                        if (!Directory.Exists(folder + @"\" + folder1 + @"\" + folder2 + @"\" + folder3))
                        {
                            Directory.CreateDirectory(folder + @"\" + folder1 + @"\" + folder2 + @"\" + folder3);
                        }
                    }
                    if (!string.IsNullOrEmpty(folder4))
                    {
                        string filePath = (folder + @"\" + folder1 + @"\" + folder2 + @"\" + folder3 + @"\" + folder4 + ".jpg").Replace("\\", @"\");
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                        try
                        {
                            OSSClientHelper.GetObject(photo.PhotoUrl, filePath);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                }
                else // 未设置文件命名方式，使用默认的方式进行下载
                {
                    if (!Directory.Exists(folder + @"\" + photo.ProjectId))
                    {
                        Directory.CreateDirectory(folder + @"\" + photo.ProjectId);//创建期号文件夹
                    }
                    if (!Directory.Exists(folder + @"\" + photo.ProjectId + @"\" + photo.ShopCode))
                    {
                        Directory.CreateDirectory(folder + @"\" + photo.ProjectId + @"\" + photo.ShopCode);//创建经销商代码文件夹
                    }
                    if (File.Exists(folder + @"\" + photo.ProjectId + @"\" + photo.ShopCode + @"\" + photo.CheckCode + "_" + photo.PhotoName + ".jpg"))
                    {
                        File.Delete(folder + @"\" + photo.ProjectId + @"\" + photo.ShopCode + @"\" + photo.CheckCode + "_" + photo.PhotoName + ".jpg");
                    }
                    try
                    {
                        OSSClientHelper.GetObject(photo.PhotoUrl, folder + @"\" + photo.ProjectId + @"\" + photo.ShopCode + @"\" + photo.CheckCode + "_" + photo.PhotoName + ".jpg");
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            // 打包文件
            if (!ZipInForFiles(list, downLoadfolder, basePath, downLoadPath, 9)) { return ""; }
            else // 压缩成功后上传到OSS
            {
                return OSSClientHelper.PutObjectMultipart("DownTempFile" + @"/" + downLoadfolder + ".zip", downLoadPath);
            }
            //  return downLoadPath.Replace(defaultPath, "");
        }
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="fileNames"></param>
        /// <param name="foler"></param>
        /// <param name="folderToZip"></param>
        /// <param name="zipedFile"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private bool ZipInForFiles(List<AnswerPhotoDto> fileNames, string foler, string folderToZip, string zipedFile, int level)
        {
            bool isSuccess = true;
            if (!Directory.Exists(folderToZip))
            {
                return false;
            }
            try
            {
                using (ZipOutputStream zipOutStream = new ZipOutputStream(System.IO.File.Create(zipedFile)))
                {
                    zipOutStream.SetLevel(level);
                    string comment = string.Empty;

                    //创建当前文件夹
                    ZipEntry entry = new ZipEntry(foler + "/"); //加上 “/” 才会当成是文件夹创建
                    zipOutStream.PutNextEntry(entry);
                    zipOutStream.Flush();

                    Crc32 crc = new Crc32();

                    foreach (AnswerPhotoDto photo in fileNames)
                    {
                        string folder1 = GetFolderName(photo.ProjectId.ToString(), "1", photo.ShopCode, photo.ShopName, "", "", photo.CheckTypeName, GetAnswerColumnListByAnswerId(photo.AnswerId.ToString(), photo.ProjectId.ToString()));
                        string folder2 = GetFolderName(photo.ProjectId.ToString(), "2", photo.ShopCode, photo.ShopName, "", "", photo.CheckTypeName, GetAnswerColumnListByAnswerId(photo.AnswerId.ToString(), photo.ProjectId.ToString()));
                        string folder3 = GetFolderName(photo.ProjectId.ToString(), "3", photo.ShopCode, photo.ShopName, "", "", photo.CheckTypeName, GetAnswerColumnListByAnswerId(photo.AnswerId.ToString(), photo.ProjectId.ToString()));
                        string folder4 = GetFolderName(photo.ProjectId.ToString(), "4", photo.ShopCode, photo.ShopName, photo.CheckCode, photo.PhotoName, photo.CheckTypeName, GetAnswerColumnListByAnswerId(photo.AnswerId.ToString(), photo.ProjectId.ToString()));
                        string photoName = "";
                        if (string.IsNullOrEmpty(folder4)) // 未设置照片命名方式，使用默认的下载方式
                        {
                            photoName = photo.ProjectId + @"\" + photo.ShopCode + @"\" + photo.CheckCode + "_" + photo.PhotoName + ".jpg";
                        }
                        else
                        {
                            photoName = (folder1 + @"\" + folder2 + @"\" + folder3 + @"\" + folder4 + ".jpg").Replace("\\", @"\");
                        }
                        string file = Path.Combine(folderToZip, foler, photoName);
                        string extension = string.Empty;
                        if (!System.IO.File.Exists(file))
                        {
                            comment += foler + "，文件：" + photoName + "不存在。\r\n";
                            continue;
                        }

                        using (FileStream fs = System.IO.File.OpenRead(Path.Combine(folderToZip, foler, photoName)))
                        {
                            byte[] buffer = new byte[fs.Length];
                            fs.Read(buffer, 0, buffer.Length);
                            entry = new ZipEntry(foler + "/" + photoName);
                            entry.DateTime = DateTime.Now;
                            entry.Size = fs.Length;
                            fs.Close();
                            crc.Reset();
                            crc.Update(buffer);
                            entry.Crc = crc.Value;
                            zipOutStream.PutNextEntry(entry);
                            zipOutStream.Write(buffer, 0, buffer.Length);
                        }
                    }

                    zipOutStream.SetComment(comment);
                    zipOutStream.Finish();
                }
            }
            catch (Exception)
            {
                isSuccess = false;
            }
            return isSuccess;
        }
        #endregion
        #region 盘点统计
        public List<AnswerShopInfoDto> GetAnswerShopInfo(string projectId, string shopCode)
        {
            if (projectId == null) projectId = "";
            if (shopCode == null) shopCode = "";

            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                    ,new SqlParameter("@ShopCode", shopCode)
                                                      };
            Type t = typeof(AnswerShopInfoDto);
            string sql = "";
            sql = @"
                    SELECT B.ProjectCode,B.ProjectName,A.ShopCode,A.ShopName,
                    CASE 
                    WHEN EXISTS(SELECT 1 FROM Answer X INNER JOIN AnswerPhoto Y ON X.AnswerId = Y.AnswerId AND X.ProjectId = A.ProjectId AND X.ShopCode = A.ShopCode)
                    THEN  '已进店'
                    ELSE '未进店'
                    END AS ShopInStatus,
                    (SELECT TOP 1 X.InDateTime FROM AnswerPhoto X INNER JOIN Answer Y ON X.AnswerId = Y.AnswerId AND Y.ProjectId = A.ProjectId AND Y.ShopCode = A.ShopCode ORDER BY X.InDateTime)
                    AS ShopInDateTime
                    ,(SELECT TOP 1 X.InDateTime FROM AnswerPhoto X INNER JOIN Answer Y ON X.AnswerId = Y.AnswerId AND Y.ProjectId = A.ProjectId AND Y.ShopCode = A.ShopCode ORDER BY X.InDateTime DESC)
                    AS ShopOutDateTime
                    FROM UserInfo A INNER JOIN Projects B ON A.ProjectId = B.ProjectId
                    WHERE 1=1";
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND A.ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                if (!string.IsNullOrEmpty(shopCode))
                {
                    shopCode = shopCode.Replace("，", ",");
                    sql += " AND A.ShopCode IN('";
                    string[] shopList = shopCode.Split(',');
                    foreach (string shop in shopList)
                    {
                        if (shop == shopList[shopList.Length - 1])
                        {
                            sql += shop + "'";
                        }
                        else
                        {
                            sql += shop + "','";
                        }
                    }
                    sql += ")";
                }
            }
            return db.Database.SqlQuery(t, sql, para).Cast<AnswerShopInfoDto>().ToList();
        }
        #endregion
        #region 清单审核
        public void SaveRecheck(Recheck recheck)
        {
            Recheck findOne = db.Recheck.Where(x => (x.AnswerId == recheck.AnswerId)).FirstOrDefault();
            if (findOne == null)
            {
                recheck.InDateTime = DateTime.Now;
                recheck.ModifyDateTime = DateTime.Now;
                db.Recheck.Add(recheck);
            }
            else
            {
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = recheck.ModifyUserId;
                findOne.RecheckStatus = recheck.RecheckStatus;
            }
            db.SaveChanges();
        }
        #endregion
        #region 任务状态
        // 保存任务状态
        public void SaveTaskStatus(TaskStatus taskStatus)
        {
            TaskStatus findOne = db.TaskStatus.Where(x => (x.TaskId == taskStatus.TaskId && x.StatusCode == taskStatus.StatusCode)).FirstOrDefault();
            if (findOne == null)
            {
                taskStatus.InDateTime = DateTime.Now;
                db.TaskStatus.Add(taskStatus);
            }
            db.SaveChanges();
        }
        // 按任务状态查询
        public List<TaskStatusDto> GetTaskStatus(string taskId, string statusCode)
        {
            if (taskId == null) taskId = "";
            if (statusCode == null) statusCode = "";

            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TaskId", taskId)
                                                        ,new SqlParameter("@StatusCode", statusCode)
                                                       };
            Type t = typeof(TaskStatusDto);
            string sql = "";
            sql = @"SELECT A.*,B.TaskCode,B.TaskName
                    FROM [TaskStatus] A INNER JOIN Task B ON A.TaskId = B.TaskId
                    WHERE 1=1 
                    ";
            if (!string.IsNullOrEmpty(taskId))
            {
                sql += " AND A.TaskId = @TaskId";
            }
            if (!string.IsNullOrEmpty(statusCode))
            {
                sql += " AND A.StatusCode = @StatusCode";
            }

            return db.Database.SqlQuery(t, sql, para).Cast<TaskStatusDto>().ToList();
        }
        #endregion
        #region 盘点码
        // 保存盘点码
        public void SaveInventoryCode(InventroyCode inventroyCode)
        {
            InventroyCode findOne = db.InventroyCode.Where(x => (x.InventroyCodeId == inventroyCode.InventroyCodeId)).FirstOrDefault();
            if (findOne == null)
            {
                inventroyCode.InDateTime = DateTime.Now;
                inventroyCode.ModifyDateTime = DateTime.Now;
                db.InventroyCode.Add(inventroyCode);
            }
            else
            {
                findOne.InventoryDayCode = inventroyCode.InventoryDayCode;
                findOne.InventoryDate = inventroyCode.InventoryDate;
                findOne.ProjectId = inventroyCode.ProjectId;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = inventroyCode.ModifyUserId;
            }
            db.SaveChanges();
        }
        // 查询盘点码
        public List<InventroyCode> GetInventoryCode(string projectId,DateTime? startDate, DateTime? endDate, string code)
        {
            if (startDate == null)
            {
                startDate = new DateTime(2000, 1, 1);
            }
            if (endDate == null)
            {
                endDate = new DateTime(3500, 1, 1);
            }
            if (projectId == null) projectId = "";
            if (code == null) code = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@StartDate", startDate)
                                                        ,new SqlParameter("@EndDate", endDate)
                                                        ,new SqlParameter("@Code", code)
                                                        ,new SqlParameter("@ProjectId", projectId)
                                                       };
            Type t = typeof(InventroyCode);
            string sql = "";
            sql = @"SELECT A.*
                    FROM [InventroyCode] A 
                    WHERE 1=1 AND A.InventoryDate BETWEEN @StartDate AND @EndDate
                    ";
            if (!string.IsNullOrEmpty(code))
            {
                sql += " AND A.InventoryDayCode = @Code";
            }
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND A.ProjectId = @ProjectId";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<InventroyCode>().ToList();
        }
        #endregion
    }
}