using com.yrtech.InventoryAPI.DTO;
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
        public List<AccountDto> LoginForMobile(string accountId, string password)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@AccountId", accountId),
                                                       new SqlParameter("@Password",password)};
            Type t = typeof(AccountDto);
            string sql = @"SELECT A.Id,A.TenantId,A.TenantName,AccountId,AccountName,A.TelNO,A.Email,A.HeadPicUrl
                            FROM UserInfo A 
                            WHERE AccountId = @AccountId AND[Password] = @Password
                            AND GETDATE()<ExpireDateTime";
            return db.Database.SqlQuery(t, sql, para).Cast<AccountDto>().ToList();
        }
    }
}