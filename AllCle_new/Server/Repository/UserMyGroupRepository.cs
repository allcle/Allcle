using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Server.IRepository;
using Server.Models;

namespace Server.Repository
{
    public class UserMyGroupRepository : IUserMyGroupRepository
    {
        private IConfiguration _config;
        private SqlConnection db;
        public UserMyGroupRepository(IConfiguration config)                                             // db 설정하는 메소드
        {
            _config = config;

            // IConfiguration 개체를 통해서 
            // appsettings.json의 데이터베이스 연결 문자열을 읽어온다. 
            db = new SqlConnection(
                _config.GetSection("ConnectionStrings").GetSection(
                    "DefaultConnection").Value);
        }

        public List<UserMyGroup> GetUserMyGroups(string _userId)
        {
            string sql = "Select * From UserMyGroup Where ID = '" + _userId + "'";
            return this.db.Query<UserMyGroup>(sql).ToList();
        }


    }
}
