using com.yrtech.InventoryAPI.Common;
using com.yrtech.InventoryAPI.DTO;
using com.yrtech.InventoryDAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

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
        public List<AnswerDto> GetShopAnswerList(string projectId, string shopId, string checkCode, string checkTypeId, string photoCheck, string addCheck)
        {
            if (projectId == null) projectId = "";
            if (shopId == null) shopId = "";
            if (checkCode == null) checkCode = "";
            if (checkTypeId == null) checkTypeId = "";
            if (photoCheck == null) photoCheck = "";
            if (addCheck == null) addCheck = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@ShopId", shopId),
                                                       new SqlParameter("@CheckCode", checkCode),
                                                       new SqlParameter("@CheckTypeId", checkTypeId),
                                                        new SqlParameter("@PhotoCheck", photoCheck),
                                                        new SqlParameter("@AddCheck", addCheck)};
            Type t = typeof(AnswerDto);
            string sql = "";
            sql = @"SELECT A.*,B.CheckTypeName
                    FROM Answer A INNER JOIN Shop C LEFT JOIN CheckType B ON A.CheckTypeId = B.CheckTypeId AND A.ProjectId = B.ProjectId
                    WHERE 1=1 ";
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND A.ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND A.ShopId = @ShopId";
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
                    sql += " AND (EXISTS(SELECT 1 FROM AnswerPhoto WHERE AnswerId =A.AnswerId AND ISNULL(PhotoNameServer,'')<>'') OR ISNULL(Remark,'')<>'') ";
                }
                else
                {
                    sql += " AND NOT EXISTS (SELECT 1 FROM AnswerPhoto WHERE AnswerId =A.AnswerId AND ISNULL(PhotoNameServer,'')<>'') AND ISNULL(Remark,'')='' ";
                }
            }
            if (!string.IsNullOrEmpty(addCheck))
            {
                sql += " AND A.AddCheck = @AddCheck";
            }
            
            sql += " Order BY A.CheckTypeId, CheckCode";
            return db.Database.SqlQuery(t, sql, para).Cast<AnswerDto>().ToList();
        }
        public List<AnswerPhotoDto> GetAnswerPhotoList(string answerId)
        {
            if (answerId == null) answerId = "";
            
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@AnswerId", answerId)
                                                      };
            Type t = typeof(AnswerPhotoDto);
            string sql = "";
            sql = @"SELECT A.ProjectId,A.ShopId,A.CheckCode,B.PhotoId,B.PhotoNameServer,C.PhotoName,B.PhotoUrl
                    FROM Answer A INNER JOIN AnswerPhoto B ON A.AnswerId = B.AnswerId
                                  INNER JOIN PhotoList C ON B.PhotoId = C.PhotoId
                    WHERE 1=1 ";
            if (!string.IsNullOrEmpty(answerId))
            {
                sql += " AND A.AnswerId = @AnswerId";
            }
            

            sql += " Order By C.PhotoId";
            return db.Database.SqlQuery(t, sql, para).Cast<AnswerPhotoDto>().ToList();
        }
        public void ImportAnswerList(string projectId, string userId, List<AnswerDto> answerList)
        {
            string sql = "";
            int projectIdInt = Convert.ToInt32(projectId);
            SqlParameter[] para = new SqlParameter[] { };
            Type t = typeof(int);
            foreach (AnswerDto answer in answerList)
            {
                sql += "DELETE Answer WHERE ShopId = " + answer.ShopId.ToString();
                CheckType checkType = db.CheckType.Where(x => (x.CheckTypeName==answer.CheckTypeName)).FirstOrDefault();

                sql += "INSERT INTO Answer(ProjectId,ShopId,CheckCode,CheckTypeId,OtherProperty,AddCheck,ModifyUserId,ModifyDateTime,InUserId,InDateTime) VALUES(";
                sql += projectId + ",";
                sql += answer.ShopId.ToString() + ",";
                sql += answer.CheckCode+ ",";
                sql += answer.CheckTypeId.ToString() + ",";
                sql += answer.OtherProperty + ",";
                sql += "N"+ ",";
                sql += userId + ",GETDATE(),";
                sql += userId + ",GETDATE(),";
                sql += " ";
            }
            db.Database.ExecuteSqlCommand(sql, para);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="answer"></param>
        public void SaveShopAnswer(AnswerDto answerDto)
        {
            Answer answer = new Answer();
            answer.AddCheck = answerDto.AddCheck;
            answer.AnswerId = answerDto.AnswerId;
            answer.CheckCode = answerDto.CheckCode;
            answer.CheckTypeId = answerDto.CheckTypeId;
            answer.InDateTime = DateTime.Now;
            answer.InUserID = answerDto.InUserID;
            answer.ModifyDateTime = DateTime.Now;
            answer.ModifyUserId = answerDto.ModifyUserId;
            answer.OtherProperty = answerDto.OtherProperty;
            answer.ProjectId = answerDto.ProjectId;
            answer.Remark = answerDto.Remark;
            answer.Score = answerDto.Score;
            answer.ShopId = answerDto.ShopId;
            Answer findOne = db.Answer.Where(x => (x.ProjectId == answer.ProjectId && x.ShopId == answer.ShopId && x.CheckCode == answer.CheckCode)).FirstOrDefault();
            if (findOne == null)
            {
                answer.InDateTime = DateTime.Now;
                answer.AddCheck = "Y";
                db.Answer.Add(answer);
            }
            else
            {
                //findOne.PhotoName = answer.PhotoName;
                findOne.Remark = answer.Remark;
                findOne.Score = answer.Score;
                findOne.OtherProperty = answer.OtherProperty;

            }
            db.SaveChanges();

            List<AnswerPhoto> answerPhotoList = new List<AnswerPhoto>();
            foreach (AnswerPhotoDto photoDto in answerDto.answerPhotoList)
            {
                Answer answerfindOne = db.Answer.Where(x => (x.ProjectId == answer.ProjectId && x.ShopId == answer.ShopId && x.CheckCode == answer.CheckCode)).FirstOrDefault();

                AnswerPhoto answerPhoto = new AnswerPhoto();
                answerPhoto.AnswerId = answerfindOne.AnswerId;
                answerPhoto.InDateTime = DateTime.Now;
                answerPhoto.InUserId = answerDto.InUserID;
                answerPhoto.ModifyDateTime = DateTime.Now;
                answerPhoto.ModifyUserId = answerDto.ModifyUserId;
                answerPhoto.PhotoId = photoDto.PhotoId;
                answerPhoto.PhotoNameServer = photoDto.PhotoNameServer;
                answerPhoto.PhotoUrl = photoDto.PhotoUrl;
                SaveShopAnswerPhoto(answerPhoto);
            }
        }
        public void DeleteShopAnswer(List<AnswerDto> answerList)
        {
            string sql = "";
            SqlParameter[] para = new SqlParameter[] { };
            Type t = typeof(int);
            foreach (AnswerDto answer in answerList)
            {
                sql += "DELETE Answer WHERE AnswerId = " + answer.AnswerId.ToString()+" ";
            }
            db.Database.ExecuteSqlCommand(sql, para);
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
                db.AnswerPhoto.Add(answerPhoto);
            }
            else
            {
                findOne.PhotoNameServer = answerPhoto.PhotoNameServer;
                findOne.PhotoUrl = answerPhoto.PhotoUrl;
                findOne.ModifyDateTime = DateTime.Now;

            }
            db.SaveChanges();
        }
    }
}