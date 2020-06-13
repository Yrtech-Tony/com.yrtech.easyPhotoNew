using com.yrtech.InventoryAPI.DTO;
using com.yrtech.InventoryDAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace com.yrtech.InventoryAPI.Service
{
    public class AccountService
    {
        com.yrtech.InventoryDAL.InventoryDAL db = new InventoryDAL.InventoryDAL();

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public List<UserInfo> Login(string projectId,string accountId, string password)
        {
            SqlParameter[] para = new SqlParameter[] {new SqlParameter("@ProjectId", projectId),
                                                       new SqlParameter("@AccountId", accountId),
                                                       new SqlParameter("@Password",password)};
            Type t = typeof(UserInfo);
            string sql = @"SELECT A.*
                            FROM UserInfo A 
                            WHERE ProjectId = @ProjectId AND ShopCode = @AccountId AND [Password] = @Password
                            AND GETDATE()<ExpireDateTime";
            return db.Database.SqlQuery(t, sql, para).Cast<UserInfo>().ToList();
        }
      
    }
}