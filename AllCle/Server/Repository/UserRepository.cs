using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using Server.IRepository;
using Server.Models;

namespace Server.Repository
{
    public class UserRepository : IUserRepository
    {
        private IConfiguration _config;
        private SqlConnection db;
        public UserRepository(IConfiguration config)                                             // db 설정하는 메소드
        {
            _config = config;

            // IConfiguration 개체를 통해서 
            // appsettings.json의 데이터베이스 연결 문자열을 읽어온다. 
            db = new SqlConnection(
                _config.GetSection("ConnectionStrings").GetSection(
                    "DefaultConnection").Value);
        }
        public List<User> GetUsers()                             //전체 모든 과목 보기
        {
            string sql = "Select * From Users Order by NO Asc";
            return this.db.Query<User>(sql).ToList();
        }

        public void PostUsers(User _user)
        {
            string sql = "Insert Into Users (Id, Password, EncryptKey) Values (@Id, @Password, @EncryptKey)";
//            string sql = "Insert Into Users (Id, Password, EncryptKey) Values (?, ?, ?)";
            db.Execute(sql, _user);
        }


        public bool GetUserId(string _id)
        {
            string sql = "Select * From Users Where Id ='" + _id + "'";
            int num = this.db.Query<User>(sql).Count();
            if (num == 1)
                return true;
            else
                return false;
        }

        public User LoginUser(string _id)
        {
            string sql = "Select * From Users Where Id = '" +_id + "'";
            return this.db.Query<User>(sql).ToList()[0];       
        }

    }
}
