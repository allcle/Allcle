using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Server.IRepository;
using Server.Models;

namespace Server.Repository
{
    public class SubjectRepository1 : ISubjectRepository1
    {
        private IConfiguration _config;
        private SqlConnection db;
        public SubjectRepository1(IConfiguration config)                                             // db 설정하는 메소드
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
            string sql = "Select * From SubjectTable$ Order by NO Asc";
            return this.db.Query<Subject>(sql).ToList();
        }

        //.. 여기부터 실험중
        public List<Subject> GetClassNumber(string classname, string classTeach, string classtime)
        {
            string[] temp_real_classTeach = classTeach.Split('_');
            string real_classTeach = "";
            for(int i = 1; i<temp_real_classTeach.Length; i++)
            {
                if (i == 1)
                {
                    real_classTeach = temp_real_classTeach[i];
                }
                else
                {
                    real_classTeach = real_classTeach + " " + temp_real_classTeach[i];
                }
            }
            string sql = "Select classnumber from SubjectTable$ Where ClassName = N'" + classname + "' and Professor = N'" + real_classTeach + "' and (Time1 = N'" + classtime + "' or Time2 = N'" + classtime + "' or Time3 = N'" + classtime + "' or Time4 = N'" + classtime + "' or Time5 = N'" + classtime + "' or Time6 = N'" + classtime + "' or Time7 = N'" + classtime + "' or Time8 = N'" + classtime + "')";
            return this.db.Query<Subject>(sql).ToList();
        }
    }
}