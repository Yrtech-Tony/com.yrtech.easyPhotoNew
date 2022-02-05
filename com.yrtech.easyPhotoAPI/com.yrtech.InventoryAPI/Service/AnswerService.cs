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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="checkCode"></param>
        /// <param name="checkTypeId"></param>
        /// <param name="photoCheck"></param>
        /// <param name="addCheck"></param>
        /// <returns></returns>
        public List<AnswerDto> GetShopAnswerListAll(string answerId, string projectId, string shopCode, string checkCode, string checkTypeId, string photoCheck, string addCheck, string key)
        {
            if (projectId == null) projectId = "";
            if (shopCode == null) shopCode = "";
            if (checkCode == null) checkCode = "";
            if (checkTypeId == null) checkTypeId = "";
            if (photoCheck == null) photoCheck = "";
            if (addCheck == null) addCheck = "";
            if (answerId == null) answerId = "";
            if (key == null) key = "";
            SqlParameter[] para = new SqlParameter[] {  new SqlParameter("@AnswerId", answerId),
                                                        new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopCode", shopCode),
                                                       new SqlParameter("@CheckCode", checkCode),
                                                       new SqlParameter("@CheckTypeId", checkTypeId),
                                                        new SqlParameter("@PhotoCheck", photoCheck),
                                                        new SqlParameter("@AddCheck", addCheck),
                                                        new SqlParameter("@Key", key)};
            Type t = typeof(AnswerDto);
            string sql = "";
            sql = @"SELECT A.AnswerId,A.ProjectId,A.ShopCode,A.ShopName,A.CheckCode,RIGHT(A.CheckCode,8) AS CheckCodeShow,A.CheckTypeId,
                    ISNULL((SELECT CheckTypeName FROM CheckType WHERE CheckTypeId = A.CheckTypeId AND ProjectId = A.ProjectId),'') AS CheckTypeName
                    ,A.Remark AS RemarkName,
                    A.AddCheck,A.ModifyUserId,A.ModifyDateTime,A.InUserId,A.InDateTime,
                    CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = A.ProjectId AND ColumnCode = 'Column1' AND UseChk = 1)
                         THEN Column1
                    END AS Column1,
                    CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = A.ProjectId AND ColumnCode = 'Column2' AND UseChk = 1)
                         THEN Column2
                    END AS Column2,
                    CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = A.ProjectId AND ColumnCode = 'Column3' AND UseChk = 1)
                         THEN Column3
                    END AS Column3,
                    CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = A.ProjectId AND ColumnCode = 'Column4' AND UseChk = 1)
                         THEN Column4
                    END AS Column4,
                    CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = A.ProjectId AND ColumnCode = 'Column5' AND UseChk = 1)
                         THEN Column5
                    END AS Column5,
                    CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = A.ProjectId AND ColumnCode = 'Column6' AND UseChk = 1)
                         THEN Column6
                    END AS Column6,
                    CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = A.ProjectId AND ColumnCode = 'Column7' AND UseChk = 1)
                         THEN Column7
                    END AS Column7,
                    CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = A.ProjectId AND ColumnCode = 'Column8' AND UseChk = 1)
                         THEN Column8
                    END AS Column8,
                    CASE WHEN EXISTS(SELECT 1 FROM ExtendColumnProject WHERE ProjectId = A.ProjectId AND ColumnCode = 'Column9' AND UseChk = 1)
                         THEN Column9
                    END AS Column9
                    FROM Answer A 
                    WHERE 1=1 ";
            if (!string.IsNullOrEmpty(answerId))
            {
                sql += " AND A.AnswerId = @AnswerId";
            }
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND A.ProjectId = @ProjectId";
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
                    sql += " AND EXISTS(SELECT 1 FROM AnswerPhoto WHERE AnswerId =A.AnswerId ) ";
                }
                else
                {
                    sql += " AND NOT EXISTS (SELECT 1 FROM AnswerPhoto WHERE AnswerId =A.AnswerId ) ";
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
                        sql += shop+"'";
                    }
                    else
                    {
                        sql += shop + "',";
                    }
                }
                sql += ")";
            }
            sql += " Order BY A.ProjectId,A.ShopCode,A.CheckTypeId, CheckCode";
            return db.Database.SqlQuery(t, sql, para).Cast<AnswerDto>().ToList();
        }
        public List<AnswerDto> GetShopAnswerListByPage(string answerId, string projectId, string shopCode, string checkCode, string checkTypeId, string photoCheck, string addCheck, string key, int pageNum, int pageCount)
        {
            int startIndex = (pageNum - 1) * pageCount;

            return GetShopAnswerListAll(answerId, projectId, shopCode, checkCode, checkTypeId, photoCheck, addCheck, key).Skip(startIndex).Take(pageCount).ToList();
        }
        public List<AnswerPhotoDto> GetAnswerPhotoList(string answerId, string projectId, string shopCode)
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
            sql = @"SELECT A.ProjectId,A.CheckCode,B.PhotoId,B.PhotoUrl,C.PhotoName,A.ShopCode,A.ShopName,ISNULL(C.MustChk,0) AS MustChk
                    FROM Answer A INNER JOIN AnswerPhoto B ON A.AnswerId = B.AnswerId
                                  INNER JOIN PhotoList C ON B.PhotoId = C.PhotoId
                    WHERE 1=1 ";
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND A.ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                sql += " AND A.ShopCode = @ShopCode";
            }
            if (!string.IsNullOrEmpty(answerId))
            {
                sql += " AND A.AnswerId = @AnswerId";
            }
            sql += " Order By B.PhotoId";
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
                sql += "N" + "',";
                sql += answer.InUserID + ",GETDATE(),";
                sql += answer.ModifyUserId + ",GETDATE())";
                sql += " ";
            }
            db.Database.ExecuteSqlCommand(sql, para);
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
                sql += "DELETE AnswerPhoto WHERE AnswerId IN(SELECT AnswerId FROM Answer WHERE ProjectId= " + projectId + " AND ShopCode = " + shop + ") ";
                sql += "DELETE Answer WHERE ProjectId = " + projectId + " AND ShopCode = " + shop;
            }
            db.Database.ExecuteSqlCommand(sql, para);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="answer"></param>
        public Answer SaveShopAnswer(Answer answer)
        {
            Answer findOne = db.Answer.Where(x => (x.AnswerId == answer.AnswerId)).FirstOrDefault();
            if (findOne == null)
            {
                answer.InDateTime = DateTime.Now;
                answer.ModifyDateTime = DateTime.Now;
                answer.AddCheck = "Y";
                answer = db.Answer.Add(answer);
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
                answer = findOne;
            }
            db.SaveChanges();
            return answer;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="answerPhoto"></param>
        public void SaveShopAnswerPhoto(AnswerPhoto answerPhoto)
        {
            if (answerPhoto.PhotoId == 0) return;
            AnswerPhoto findOne = db.AnswerPhoto.Where(x => (x.AnswerId == answerPhoto.AnswerId && x.PhotoId == answerPhoto.PhotoId)).FirstOrDefault();
            if (findOne == null)
            {
                answerPhoto.InDateTime = DateTime.Now;
                answerPhoto.ModifyDateTime = DateTime.Now;
                db.AnswerPhoto.Add(answerPhoto);
            }
            else
            {
                findOne.PhotoUrl = answerPhoto.PhotoUrl;
                findOne.ModifyDateTime = DateTime.Now;
                findOne.ModifyUserId = answerPhoto.ModifyUserId;
            }
            db.SaveChanges();
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
                //string photoName = photo.ProjectId + "_" + photo.ShopCode + "_" + photo.PhotoName;
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
            // 打包文件
            if (!ZipInForFiles(list, downLoadfolder, basePath, downLoadPath, 9)) return "";

            return downLoadPath.Replace(defaultPath, "");
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
        private static bool ZipInForFiles(List<AnswerPhotoDto> fileNames, string foler, string folderToZip, string zipedFile, int level)
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
                        string photoName = photo.ProjectId + @"\" + photo.ShopCode + @"\" + photo.CheckCode + "_" + photo.PhotoName + ".jpg";
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
    }
}