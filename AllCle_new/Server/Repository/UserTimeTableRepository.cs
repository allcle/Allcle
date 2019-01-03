using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Server.IRepository;
using Server.Models;

namespace Server.Repository
{
    public class UserTimeTableRepository : IUserTimeTableRepository
    {
        private IConfiguration _config;
        private SqlConnection db;
        public UserTimeTableRepository(IConfiguration config)                                             // db 설정하는 메소드
        {
            _config = config;

            // IConfiguration 개체를 통해서 
            // appsettings.json의 데이터베이스 연결 문자열을 읽어온다. 
            db = new SqlConnection(
                _config.GetSection("ConnectionStrings").GetSection(
                    "DefaultConnection").Value);
        }

        public List<UserTimeTable> GetUserTimeTables(string _userId)
        {
            string sql = "Select * From UserTimeTable Where ID = '" + _userId + "'";
            return this.db.Query<UserTimeTable>(sql).ToList();
        }

        public void PostTimeTalbe(UserTimeTable _userTimeTable)
        {
            string sql = "Insert Into UserTimeTable (ID, TimeTableName) Values (@ID, @TimeTalbeName)";
            db.Execute(sql, _userTimeTable);
        }
    }
}
