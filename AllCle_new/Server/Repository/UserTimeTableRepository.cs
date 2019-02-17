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
            string sql = "Select * From UserTimeTable Where ID = N'" + _userId + "'";
            return this.db.Query<UserTimeTable>(sql).ToList();
        }

        public void PostTimeTable(UserTimeTable _userTimeTable)
        {
            string sql = "Insert Into UserTimeTable (ID, TimeTableName, SaveTime, EditTime) Values (@ID, @TimeTableName, @SaveTime, @EditTime)";
            db.Execute(sql, _userTimeTable);
        }
        /*
        public void UpdateTimeTable(string EditTime, string ID, string TimeTableName)
        {
            // 시간표 다시 저장할 때 쓰는 쿼리
            string sql = "Update UserTimeTable SET EditTime = '" + EditTime + "' Where ID = '" + ID + "' and TimeTableName = N'" + TimeTableName + "'";
            db.Execute(sql);
        }
        */

        public void UpdateTimeTable(UserTimeTable _userTimeTable)
        {
            // 시간표 다시 저장할 때 쓰는 쿼리
            string sql = "Update UserTimeTable SET EditTime = '" + _userTimeTable.EditTime + "' Where ID = '" + _userTimeTable.ID + "' and TimeTableName = N'" + _userTimeTable.TimeTableName + "'";
            db.Execute(sql, _userTimeTable);
        }

        public void UpdateTimeTableName(UpdateTImeTableName updateTImeTableName)
        {
            // 시간표 이름 수정할 때 쓰는 쿼리
            string sql = "UPDATE UserTimeTable SET TimeTableName = N'" + updateTImeTableName.NewTiemTableName + "' WHERE ID = '" + updateTImeTableName.ID + "' AND TimeTableName = N'" + updateTImeTableName.OldTimeTableName + "'";
            db.Execute(sql, updateTImeTableName);
        }

        public List<UserTimeTable> CheckSaveTimeTableName(string _id, string TimeTableName)
        {
            string sql = "Select SaveTime from UserTimeTable Where ID = '" + _id + "' and TimeTableName = N'" + TimeTableName + "'";
            return this.db.Query<UserTimeTable>(sql).ToList();
        }
        public void DeleteTimeTable(Del del)
        {
            string sql = "DELETE FROM UserTimeTable WHERE ID = '" + del.ID + "' AND MyGroupName = N'" + del.Name + "'";
            db.Execute(sql);
        }
    }
}
