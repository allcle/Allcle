using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Server.IRepository;
using Server.Models;

namespace Server.Repository
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

        public List<string> GetTimeTableClassNumbers(string _id, string TimeTableName)
        {
            string sql = "Select ClassNumber " +
                         "From TimeTableClassNumber " +
                         "Where TimeTableName = '" + TimeTableName + "' " +
                         "and ID = N'" + _id + "'";
            return db.Query<string>(sql).ToList();
        }


        public void PostTimeTable(TimeTableClassNumber _timeTableClassNumber)
        {
            string sql = "Insert Into UserTimeTable (ID, TimeTableClassNumber, ClassNumber) Values (@ID, @TimeTableClassNumber, @ClassNumber)";
            db.Execute(sql, _timeTableClassNumber);
        }
        
        /*public void UpdateTimeTableClassNumber(UpdateUserIdTimeTable _timeTableClassNumber)
        {
            string sql = "UPDATE TimeTableClassNumber SET TimeTableName = '" + _timeTableClassNumber.TimeTableName + "' WHERE ID = '" + _timeTableClassNumber.ID + "' and TimeTableName = N'" + _timeTableClassNumber.OldTimeTableName + "'";
            db.Execute(sql, _timeTableClassNumber);
        }*/
    }
}
