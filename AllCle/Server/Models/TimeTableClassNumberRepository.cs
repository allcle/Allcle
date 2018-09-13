using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class TimeTableClassNumberRepository : ITimeTableClassNumberRepository
    {
        private IConfiguration _config;
        private SqlConnection db;
        public TimeTableClassNumberRepository(IConfiguration config)                                             // db 설정하는 메소드
        {
            _config = config;

            // IConfiguration 개체를 통해서 
            // appsettings.json의 데이터베이스 연결 문자열을 읽어온다. 
            db = new SqlConnection(
                _config.GetSection("ConnectionStrings").GetSection(
                    "DefaultConnection").Value);
        }

        public List<TimeTableClassNumber> GetTimeTableClassNumbers(string _timeTableName)
        {
            string sql = "Select * From TimeTableClassNumber Where TimeTableName = N'" + _timeTableName + "'";
            return db.Query<TimeTableClassNumber>(sql).ToList();
        }


        public void PostTimeTable(TimeTableClassNumber _timeTableClassNumber)
        {
            string sql = "Insert Into UserTimeTable (TimeTableName, TimeTableClassNumber) Values (@TimeTableName, @TimeTableClassNumber)";
            db.Execute(sql, _timeTableClassNumber);
        }
    }
}
