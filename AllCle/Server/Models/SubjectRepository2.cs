using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Server.Models
{
    public class SubjectRepository2 : ISubjectRepository2
    {
        private IConfiguration _config;
        private SqlConnection db;
        public SubjectRepository2(IConfiguration config)                                             // db 설정하는 메소드
        {
            _config = config;

            // IConfiguration 개체를 통해서 
            // appsettings.json의 데이터베이스 연결 문자열을 읽어온다. 
            db = new SqlConnection(
                _config.GetSection("ConnectionStrings").GetSection(
                    "DefaultConnection").Value);
        }

        
        public List<Subject> GetSubjects()                             //전체 모든 과목 보기
        {
            string sql = "Select * From SubjectTable2$ Order by NO Asc";  
            return this.db.Query<Subject>(sql).ToList();
        }
                
        /*
        // 입력
        public string AddSubject(Subject model)                                                     // 로그인할 때 사용하는 메소드. (인수 생각)
                                                                                                    // 로그인할 때 쓰는 거라면, 계정 DB와 과목DB 구분해야되니까, 이건 계정 DB 연결되있는 다른 Class 만들어서 거기서 구현해야되지 않을까?
        {
            string sql = "Insert Into Subjects (Id, Pw) Values (@Id, @Pw)";
            var id = this.db.Execute(sql, model);
            return "successfully add";
        }*/

    }
}
