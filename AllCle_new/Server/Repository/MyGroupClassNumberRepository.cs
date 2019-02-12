using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Server.Models;
using Dapper;
using Server.IRepository;

namespace Server.Repository
{
    public class MyGroupClassNumberRepository : IMyGroupClassNumberRepository
    {
        private IConfiguration _config;
        private SqlConnection db;
        public MyGroupClassNumberRepository(IConfiguration config)                                             // db 설정하는 메소드
        {
            _config = config;

            // IConfiguration 개체를 통해서 
            // appsettings.json의 데이터베이스 연결 문자열을 읽어온다. 
            db = new SqlConnection(
                _config.GetSection("ConnectionStrings").GetSection(
                    "DefaultConnection").Value);
        }

        public List<string> GetMyGroupClassNumbers(string _id, string MyGroupName)
        {
            string sql = "Select ClassNumber " +
                         "From MyGroupClassNumber " +
                         "Where MyGroupName = N'" + MyGroupName + "' " +
                         "and ID = N'" + _id + "'";
            return db.Query<string>(sql).ToList();
        }

        public void PostSubjectToMyGroup(MyGroupClassNumber _myGroupClassNumber)
        {
            string sql = "Insert Into MyGroupClassNumber (ID, MyGroupName, ClassNumber) Values (@ID, @MyGroupName, @ClassNumber)";
            db.Execute(sql, _myGroupClassNumber);
        }

        public bool GetClassNumber(string ID, string MyGroupName, string ClassNumber)
        {
            string sql = "Select * From MyGroupClassNumber Where Id ='" + ID + "' and MyGroupName ='" + MyGroupName + "' and ClassNumber = '" + ClassNumber + "'";
            int num = this.db.Query<MyGroupClassNumber>(sql).Count();
            if (num == 1)
                return true;
            else
                return false;
        }
    }
}
