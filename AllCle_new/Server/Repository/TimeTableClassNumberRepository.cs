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
            string sql = "Select ClassNumber From TimeTableClassNumber Where TimeTableName = N'" + TimeTableName + "' and ID = '" + _id + "'";
            return db.Query<string>(sql).ToList();
        }

        public void PostTimeTable(TimeTableClassNumber _timeTableClassNumber)
        {
            string sql = "Insert Into TimeTableClassNumber (ID, TimeTableName, ClassNumber) Values (@ID, @TimeTableName, @ClassNumber)";
            db.Execute(sql, _timeTableClassNumber);
        }

        public bool GetClassNumber(string ID, string TimeTableName, string ClassNumber)
        {
            string sql = "Select * From TimeTableClassNumber Where Id ='" + ID + "' and TimeTableName = N'" + TimeTableName + "' and ClassNumber = '" + ClassNumber + "'";
            int num = this.db.Query<User>(sql).Count();
            if (num == 1)
                return true;
            else
                return false;
        }

        public List<TimeTableClassNumber> GetClassNumber(string ID, string TimeTableName)
        {
            string sql = "Select ClassNumber From TimeTableClassNumber Where ID = '" + ID + "' and TimeTableName = N'" + TimeTableName + "'";
            return this.db.Query<TimeTableClassNumber>(sql).ToList();
        }

        public void DeleteSubjectInTimeTable(TimeTableClassNumber timeTableClassNumber)
        {
            string sql = "DELETE FROM TimeTableClassNumber WHERE ID = '" + timeTableClassNumber.ID + "' AND TimeTableName = N'" + timeTableClassNumber.TimeTableName + "' AND ClassNumber = '" + timeTableClassNumber.ClassNumber + "'";
            db.Execute(sql, timeTableClassNumber);
        }

        /*public void UpdateTimeTableClassNumber(UpdateUserIdTimeTable _timeTableClassNumber)
        {
            string sql = "UPDATE TimeTableClassNumber SET TimeTableName = '" + _timeTableClassNumber.TimeTableName + "' WHERE ID = '" + _timeTableClassNumber.ID + "' and TimeTableName = N'" + _timeTableClassNumber.OldTimeTableName + "'";
            db.Execute(sql, _timeTableClassNumber);
        }*/
    }
}
