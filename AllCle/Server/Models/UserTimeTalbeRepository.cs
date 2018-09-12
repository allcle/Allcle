using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class UserTimeTalbeRepository : IUserTimeTableRepository
    {        
        private IConfiguration _config;
        private SqlConnection db;
        public UserTimeTalbeRepository(IConfiguration config)                                             // db 설정하는 메소드
        {
            _config = config;

            // IConfiguration 개체를 통해서 
            // appsettings.json의 데이터베이스 연결 문자열을 읽어온다. 
            db = new SqlConnection(
                _config.GetSection("ConnectionStrings").GetSection(
                    "DefaultConnection").Value);
        }        

        public void PostTimeTalbe(UserTimeTable _userTimeTable)
        {
            string sql = "Insert Into UserTimeTable (ID, TimeTableName) Values (@ID, @TimeTalbeName)";
            db.Execute(sql, _userTimeTable);
        }
    }
}
