using com.yrtech.InventoryAPI.Common;
using com.yrtech.InventoryDAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace com.yrtech.InventoryAPI.Service
{
    public class AnswerService
    {
        com.yrtech.InventoryDAL.InventoryDAL db = new InventoryDAL.InventoryDAL();
        MasterService masterService = new MasterService();
        AccountService accountService = new AccountService();

        /// <summary>
        /// </summary>
        /// <param name="projectCode"></param>
        /// <param name="shopCode"></param>
        /// <param name="allChk"></param>
        /// <param name="vinCode"></param>
        /// <returns></returns>
        public List<Answer> GetShopAnswerList(string projectId, string shopId, string checkCode, string checkTypeId, string photoCheck, string addCheck)
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
            Type t = typeof(Answer);
            string sql = "";
            sql = @"SELECT * 
                    FROM Answer WHERE 1=1 ";
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND ProjectId = @ProjectId";
            }
            if (!string.IsNullOrEmpty(shopId))
            {
                sql += " AND ShopId = @ShopId";
            }
            if (!string.IsNullOrEmpty(checkCode))
            {
                sql += " AND CheckCode LIKE '%'+@CheckCode+'%'";
            }
            if (!string.IsNullOrEmpty(checkTypeId))
            {
                sql += " AND CheckTypeId = @CheckTypeId";
            }
            if (!string.IsNullOrEmpty(photoCheck))
            {
                if (photoCheck == "Y")
                {
                    sql += " AND (ISNULL(PhotoName,'')<>'' OR ISNULL(Remark,'')<>'' ";
                }
                else
                {
                    sql += " AND (ISNULL(PhotoName,'')='' AND ISNULL(Remark,'')='' ";
                }
            }
            if (!string.IsNullOrEmpty(addCheck))
            {
                sql += "AND AddCheck = @AddCheck";
            }
            
            sql += " Order By CheckTypeId, CheckCode";
            return db.Database.SqlQuery(t, sql, para).Cast<Answer>().ToList();
        }
        public void SaveShopAnswer(Answer answer)
        {
            if (answer.Remark == "请选择") answer.Remark = "";
            // CommonHelper.log("ProjectCode:" + answer.ProjectCode + "ShopCode" + answer.ShopCode + "VinCode" + answer.VinCode + "Remark" + answer.Remark + "PhotoName" + answer.PhotoName );
            Answer findOne = db.Answer.Where(x => (x.ProjectId == answer.ProjectId && x.ShopId == answer.ShopId && x.CheckCode == answer.CheckCode)).FirstOrDefault();
            if (findOne == null)
            {
                answer.InDateTime = DateTime.Now;
                answer.AddCheck = "Y";
                db.Answer.Add(answer);
            }
            else
            {
                findOne.PhotoName = answer.PhotoName;
                findOne.Remark = answer.Remark;
                findOne.Score = answer.Score;
                findOne.OtherProperty = answer.OtherProperty;

            }
            db.SaveChanges();
        }
    }
}