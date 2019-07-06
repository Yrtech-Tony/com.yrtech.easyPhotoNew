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
        public List<Projects> GetProject(string tenantId,string projectId)
        {
            if (tenantId == null) tenantId = "";
            if (projectId == null) projectId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@TenantId", tenantId),
                                                        new SqlParameter("@ProjectId", projectId)};
            Type t = typeof(Projects);
            string sql = "";
            sql = @"SELECT * FROM Projects
                    WHERE 1=1 AND GETDATE()<ExpireDateTime
                    ";
            if (!string.IsNullOrEmpty(tenantId))
            {
                sql += " AND TenantId = @TenantId";
            }
            if (!string.IsNullOrEmpty(projectId))
            {
                sql += " AND ProjectId = @ProjectId";
            }
            sql += " ORDER BY OrderNO DESC";
            return db.Database.SqlQuery(t, sql, para).Cast<Projects>().ToList();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="shopId"></param>
        /// <param name="shopCode"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public List<UserInfoShop> GetUserInfoShop(string projectId,string shopId,string shopCode,string userId)
        {
            if(shopCode==null)shopCode="";
            if (shopId == null) shopId = "";
            if (projectId == null) projectId = "";
            if (userId == null) userId = "";
            SqlParameter[] para = new SqlParameter[] {new SqlParameter("@ProjectId", projectId),
                                                    new SqlParameter("@ShopId", shopId),
                                                    new SqlParameter("@ShopCode", shopCode),
                                                    new SqlParameter("@UserId", userId)};
            Type t = typeof(UserInfoShop);
            string sql = "";
            sql = @"SELECT 
                       *
                      FROM [UserInfoShop]
                    WHERE  1=1 ";
          
            if (!string.IsNullOrEmpty(shopCode))
            {
                sql += " AND ShopCode = @ShopCode";
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
        /// <param name="projectId"></param>
        /// <param name="checkTypeId"></param>
        /// <returns></returns>
        public List<Note> GetNote(string projectId,string checkTypeId,string addCheck)
        {
            if (projectId == null) projectId = "";
            if (checkTypeId == null) checkTypeId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                        , new SqlParameter("@CheckTypeId", checkTypeId)
                                                        , new SqlParameter("@AddCheck", addCheck)};
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
            return db.Database.SqlQuery(t, sql, para).Cast<Note>().ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="checkTypeId"></param>
        /// <returns></returns>
        public List<CheckType> GetCheckType(string projectId, string checkTypeId)
        {
            if (projectId == null) projectId = "";
            if (checkTypeId == null) checkTypeId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                        , new SqlParameter("@CheckTypeId", checkTypeId) };
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
            return db.Database.SqlQuery(t, sql, para).Cast<CheckType>().ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="otherType"></param>
        /// <param name="otherCode"></param>
        /// <returns></returns>
        public List<OtherProperty> GetOtherProperty(string projectId, string otherType,string otherCode)
        {
            if (projectId == null) projectId = "";
            if (otherType == null) otherType = "";
            if(otherCode == null) otherCode = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                        , new SqlParameter("@OtherType", otherType)
                                                        , new SqlParameter("@OtherCode", otherCode)};
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
                sql += " AND OtherType = @otherType";
            }
            if (!string.IsNullOrEmpty(otherCode))
            {
                sql += " AND OtherCode = @otherCode";
            }
            return db.Database.SqlQuery(t, sql, para).Cast<OtherProperty>().ToList();
        }
        public List<OtherPropertyTypeDto> GetOtherPropertyType(string projectId)
        {
            if (projectId == null) projectId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)};
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
        public List<PhotoList> GetPhotoList(string projectId, string checkTypeId)
        {
            if (projectId == null) projectId = "";
            if (checkTypeId == null) checkTypeId = "";
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectId", projectId)
                                                        , new SqlParameter("@CheckTypeId", checkTypeId) };
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
            return db.Database.SqlQuery(t, sql, para).Cast<PhotoList>().ToList();
        }
    }
}