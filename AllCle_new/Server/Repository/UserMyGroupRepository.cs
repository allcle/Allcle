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
            string sql = "Select * From UserMyGroup Where ID = '" + _userId + "' Order by SaveTime";
            return this.db.Query<UserMyGroup>(sql).ToList();
        }

        public void PostMyGroup(UserMyGroup _userMyGroup)
        {
            string sql = "Insert Into UserMyGroup (ID, MyGroupName, SaveTime, EditTime) Values (@ID, @MyGroupName, @SaveTime, @EditTime)";
            db.Execute(sql, _userMyGroup);
        }

        public void UpdateMyGroup(UserMyGroup _userMyGroup)
        {
            // 시간표 다시 저장할 때 쓰는 쿼리
            string sql = "Update UserMyGroup SET EditTime = '" + _userMyGroup.EditTime + "' Where ID = '" + _userMyGroup.ID + "' and MyGroupName = N'" + _userMyGroup.MyGroupName + "'";
            db.Execute(sql, _userMyGroup);
        }

        public List<UserMyGroup> CheckSaveMyGroupName(string _id, string MyGroupName)
        {
            string sql = "Select SaveTime from UserMyGroup Where ID = '" + _id + "' and MyGroupName = N'" + MyGroupName + "'";
            return this.db.Query<UserMyGroup>(sql).ToList();
        }
        
        public void DeleteMyGroup(Del del)
        {
            string sql = "DELETE FROM UserMyGroup WHERE ID = '"+ del .ID + "' AND MyGroupName = N'" + del.Name + "'";
            db.Execute(sql,del);
        }




    }
}
