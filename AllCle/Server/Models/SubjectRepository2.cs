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

        public List<Subject> GetTimeOnSubjectOnSearchOff(string _time, string _subject)  //남은시간에서만, 담은과목 제외
        {
            string[] timeParser = new string[60];
            string[] subjectParser = new string[10];
            int index = 0, numOfTime = 0, numOfSubject = 0;
            for (numOfTime = 0; _time.Contains("&"); numOfTime++)                                                            // daylist string을 "|"를 기준으로 다시 나눈뒤, 과목시간 데이터를 배열원소로 저장한다.
            {
                index = _time.IndexOf("&");
                timeParser[numOfTime] = _time.Substring(0, index);
                _time = _time.Remove(0, index + 1);
            }
            for (numOfSubject = 0; _subject.Contains("&"); numOfSubject++)
            {
                index = _subject.IndexOf("&");
                subjectParser[numOfSubject] = _subject.Substring(0, index);
                _subject = _subject.Remove(0, index + 1);
            }
            string sql = "Select * From SubjectTable2$ Where Time1 <> N'" + timeParser[0] + "' ";
            for (int j = 1; j < numOfTime; j++)
            {
                sql = sql + "and Time1 <> N'" + timeParser[j] + "' ";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time2 <> N'" + timeParser[j] + "' ";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time3 <> N'" + timeParser[j] + "' ";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time4 <> N'" + timeParser[j] + "' ";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time5 <> N'" + timeParser[j] + "'";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time6 <> N'" + timeParser[j] + "'";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time7 <> N'" + timeParser[j] + "'";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time8 <> N'" + timeParser[j] + "'";
            }
            for (int j = 0; j < numOfSubject; j++)
            {
                sql = sql + "and ClassName <> N'" + subjectParser[j] + "'";
            }
            return this.db.Query<Subject>(sql).ToList();
        }     
        public List<Subject> GetTimeOnSubjectOffSearchOff(string _time)                //남은 시간만에서
        {
            string[] timeParser = new string[60];
            int index = 0, numOfTime = 0;
            for (numOfTime = 0; _time.Contains("&"); numOfTime++)                                                            // daylist string을 "|"를 기준으로 다시 나눈뒤, 과목시간 데이터를 배열원소로 저장한다.
            {
                index = _time.IndexOf("&");
                timeParser[numOfTime] = _time.Substring(0, index);
                _time = _time.Remove(0, index + 1);
            }            
            string sql = "Select * From SubjectTable2$ Where Time1 <> N'" + timeParser[0] + "' ";                    // 위에서 배열원소로 저장한 과목시간 데이터를 따로 출력해준다.
            for (int j = 1; j < numOfTime; j++)
            {
                sql = sql + "and Time1 <> N'" + timeParser[j] + "' ";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time2 <> N'" + timeParser[j] + "' ";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time3 <> N'" + timeParser[j] + "' ";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time4 <> N'" + timeParser[j] + "' ";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time5 <> N'" + timeParser[j] + "'";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time6 <> N'" + timeParser[j] + "'";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time7 <> N'" + timeParser[j] + "'";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time8 <> N'" + timeParser[j] + "'";
            }            
            return this.db.Query<Subject>(sql).ToList();
        }
        public List<Subject> GetTimeOffSubjectOnSearchOff(string _subject)                //담은 과목만 제외
        {
            string[] subjectParser = new string[10];
            int index = 0, numOfSubject = 0;
            for (numOfSubject = 0; _subject.Contains("&"); numOfSubject++)
            {
                index = _subject.IndexOf("&");
                subjectParser[numOfSubject] = _subject.Substring(0, index);
                _subject = _subject.Remove(0, index + 1);
            }
            string sql = "Select * From SubjectTable2$ Where ClassName <> N'" + subjectParser[0] + "' ";
            for (int j = 0; j < numOfSubject; j++)
            {
                sql = sql + "and ClassName <> N'" + subjectParser[j] + "'";
            }
            return this.db.Query<Subject>(sql).ToList();
        }
        public List<Subject> GetTimeOffSubjectOffSearchOff()                             //전체 모든 과목 보기
        {
            string sql = "Select * From SubjectTable2$ Order by NO Asc";  
            return this.db.Query<Subject>(sql).ToList();
        }

        public List<Subject> GetTimeOnSubjectOnSearchOn(string _time, string _subject, string _search) //남은시간에서만, 담은과목제외, 검색
        {
            string[] timeParser = new string[60];
            string[] subjectParser = new string[10];
            int index = 0, numOfTime = 0, numOfSubject = 0;
            for (numOfTime = 0; _time.Contains("&"); numOfTime++)                                                            // daylist string을 "|"를 기준으로 다시 나눈뒤, 과목시간 데이터를 배열원소로 저장한다.
            {
                index = _time.IndexOf("&");
                timeParser[numOfTime] = _time.Substring(0, index);
                _time = _time.Remove(0, index + 1);
            }
            for (numOfSubject = 0; _subject.Contains("&"); numOfSubject++)
            {
                index = _subject.IndexOf("&");
                subjectParser[numOfSubject] = _subject.Substring(0, index);
                _subject = _subject.Remove(0, index + 1);
            }
            string sql = "Select * From SubjectTable2$ Where Time1 <> N'" + timeParser[0] + "' ";
            for (int j = 1; j < numOfTime; j++)
            {
                sql = sql + "and Time1 <> N'" + timeParser[j] + "' ";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time2 <> N'" + timeParser[j] + "' ";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time3 <> N'" + timeParser[j] + "' ";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time4 <> N'" + timeParser[j] + "' ";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time5 <> N'" + timeParser[j] + "'";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time6 <> N'" + timeParser[j] + "'";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time7 <> N'" + timeParser[j] + "'";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time8 <> N'" + timeParser[j] + "'";
            }
            for (int j = 0; j < numOfSubject; j++)
            {
                sql = sql + "and ClassName <> N'" + subjectParser[j] + "'";
            }
            sql = sql + "and ClassName Like N'%" + _search + "%'";
            return this.db.Query<Subject>(sql).ToList();
        }
        public List<Subject> GetTimeOnSubjectOffSearchOn(string _time, string _search)              //담은 과목 제외하고 검색
        {
            string[] timeParser = new string[60];
            int index = 0, numOfTime = 0;
            for (numOfTime = 0; _time.Contains("&"); numOfTime++)                                                            // daylist string을 "|"를 기준으로 다시 나눈뒤, 과목시간 데이터를 배열원소로 저장한다.
            {
                index = _time.IndexOf("&");
                timeParser[numOfTime] = _time.Substring(0, index);
                _time = _time.Remove(0, index + 1);
            }
            string sql = "Select * From SubjectTable2$ Where Time1 <> N'" + timeParser[0] + "' ";                    // 위에서 배열원소로 저장한 과목시간 데이터를 따로 출력해준다.
            for (int j = 1; j < numOfTime; j++)
            {
                sql = sql + "and Time1 <> N'" + timeParser[j] + "' ";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time2 <> N'" + timeParser[j] + "' ";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time3 <> N'" + timeParser[j] + "' ";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time4 <> N'" + timeParser[j] + "' ";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time5 <> N'" + timeParser[j] + "'";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time6 <> N'" + timeParser[j] + "'";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time7 <> N'" + timeParser[j] + "'";
            }
            for (int j = 0; j < numOfTime; j++)
            {
                sql = sql + "and Time8 <> N'" + timeParser[j] + "'";
            }
            sql = sql + "and ClassName Like N'%" + _search + "%'";
            return this.db.Query<Subject>(sql).ToList();
        }  
        public List<Subject> GetTimeOffSubjectOnSearchOn(string _subject, string _search)                 //남은 시간에서만 검색
        {
            string[] subjectParser = new string[10];
            int index = 0, numOfSubject = 0;
            for (numOfSubject = 0; _subject.Contains("&"); numOfSubject++)
            {
                index = _subject.IndexOf("&");
                subjectParser[numOfSubject] = _subject.Substring(0, index);
                _subject = _subject.Remove(0, index + 1);
            }
            string sql = "Select * From SubjectTable2$ Where ClassName <> N'" + subjectParser[0] + "' ";
            for (int j = 0; j < numOfSubject; j++)
            {
                sql = sql + "and ClassName <> N'" + subjectParser[j] + "'";
            }
            sql = sql + "and ClassName Like N'%" + _search + "%'";
            return this.db.Query<Subject>(sql).ToList();
        }
        public List<Subject> GetTimeOffSubjectOffSearchOn(string _search)                              //그냥 과목검색
        {
            string sql = "Select * From SubjectTable2$ Where ClassName Like N'%" + _search + "%' Order by NO Asc";     
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
        }

        public List<Subject> GetSubjects()                                                          // 기본 출력화면
        {
            string sql = "Select * From SubjectTable2$ Order by NO Asc";
            return this.db.Query<Subject>(sql).ToList();
        }
        public List<Subject> GetOnlySubjects(string _subjectName)                                   // 클라이언트에서 과목명 검색할 때 사용
        {
            string sql = "Select * From SubjectTable2$ Where ClassName Like N'%" + _subjectName + "%'";
            return this.db.Query<Subject>(sql).ToList();
        }

        public List<Subject> GetSubjects(string _daylist)                                           // daylist = 과목 시간을 "|"를 기준으로 merge한 string
        {
            string[] parser = new string[60];
            int index = 0, i = 0;
            for (i = 0; _daylist.Contains("&"); i++)                                                // daylist string을 "|"를 기준으로 다시 나눈뒤, 과목시간 데이터를 배열원소로 저장한다.
            {
                index = _daylist.IndexOf("&");
                parser[i] = _daylist.Substring(0, index);
                _daylist = _daylist.Remove(0, index + 1);
            }
            string sql = "Select * From SubjectTable2$ Where Time1 <> N'" + parser[0] + "' ";           // 위에서 배열원소로 저장한 과목시간 데이터를 따로 출력해준다.
            for (int j = 1; j < i; j++)
            {
                sql = sql + "and Time1 <> N'" + parser[j] + "' ";
            }
            for (int j = 0; j < i; j++)
            {
                sql = sql + "and Time2 <> N'" + parser[j] + "' ";
            }
            for (int j = 0; j < i; j++)
            {
                sql = sql + "and Time3 <> N'" + parser[j] + "' ";
            }
            for (int j = 0; j < i; j++)
            {
                sql = sql + "and Time4 <> N'" + parser[j] + "' ";
            }
            for (int j = 0; j < i; j++)
            {
                sql = sql + "and Time5 <> N'" + parser[j] + "'";
            }
            for (int j = 0; j < i; j++)
            {
                sql = sql + "and Time6 <> N'" + parser[j] + "'";
            }
            for (int j = 0; j < i; j++)
            {
                sql = sql + "and Time7 <> N'" + parser[j] + "'";
            }
            for (int j = 0; j < i; j++)
            {
                sql = sql + "and Time8 <> N'" + parser[j] + "'";
            }
            return this.db.Query<Subject>(sql).ToList();
        }
        public List<Subject> GetSubjects(string _daylist, string _subjectName)                                  // daylist = 과목 시간을 "|"를 기준으로 merge한 string
        {
            string[] parser = new string[60];
            int index = 0, i = 0;
            for (i = 0; _daylist.Contains("&"); i++)                                                            // daylist string을 "|"를 기준으로 다시 나눈뒤, 과목시간 데이터를 배열원소로 저장한다.
            {
                index = _daylist.IndexOf("&");
                parser[i] = _daylist.Substring(0, index);
                _daylist = _daylist.Remove(0, index + 1);
            }
            string sql = "Select * From SubjectTable2$ Where Time1 <> N'" + parser[0] + "' ";                    // 위에서 배열원소로 저장한 과목시간 데이터를 따로 출력해준다.
            for (int j = 1; j < i; j++)
            {
                sql = sql + "and Time1 <> N'" + parser[j] + "' ";
            }
            for (int j = 0; j < i; j++)
            {
                sql = sql + "and Time2 <> N'" + parser[j] + "' ";
            }
            for (int j = 0; j < i; j++)
            {
                sql = sql + "and Time3 <> N'" + parser[j] + "' ";
            }
            for (int j = 0; j < i; j++)
            {
                sql = sql + "and Time4 <> N'" + parser[j] + "' ";
            }
            for (int j = 0; j < i; j++)
            {
                sql = sql + "and Time5 <> N'" + parser[j] + "'";
            }
            for (int j = 0; j < i; j++)
            {
                sql = sql + "and Time6 <> N'" + parser[j] + "'";
            }
            for (int j = 0; j < i; j++)
            {
                sql = sql + "and Time7 <> N'" + parser[j] + "'";
            }
            for (int j = 0; j < i; j++)
            {
                sql = sql + "and Time8 <> N'" + parser[j] + "'";
            }
            sql = sql + "and ClassName Like N'%" + _subjectName + "%'";
            return this.db.Query<Subject>(sql).ToList();
        }
        public List<Subject> GetFilter(string _daylist, string _subjectName, string _subjectInTable)
        {
            string[] dayParser = new string[60];
            string[] subjectParser = new string[60];
            int index = 0, i = 0, k = 0;
            for (i = 0; _daylist.Contains("&"); i++)                                                            // daylist string을 "|"를 기준으로 다시 나눈뒤, 과목시간 데이터를 배열원소로 저장한다.
            {
                index = _daylist.IndexOf("&");
                dayParser[i] = _daylist.Substring(0, index);
                _daylist = _daylist.Remove(0, index + 1);
            }
            for (k = 0; _subjectInTable.Contains("&"); k++)
            {
                index = _subjectInTable.IndexOf("&");
                subjectParser[k] = _subjectInTable.Substring(0, index);
                _subjectInTable = _subjectInTable.Remove(0, index + 1);
            }
            string sql = "Select * From SubjectTable2$ Where Time1 <> N'" + dayParser[0] + "' ";                    // 위에서 배열원소로 저장한 과목시간 데이터를 따로 출력해준다.
            for (int j = 1; j < i; j++)
            {
                sql = sql + "and Time1 <> N'" + dayParser[j] + "' ";
            }
            for (int j = 0; j < i; j++)
            {
                sql = sql + "and Time2 <> N'" + dayParser[j] + "' ";
            }
            for (int j = 0; j < i; j++)
            {
                sql = sql + "and Time3 <> N'" + dayParser[j] + "' ";
            }
            for (int j = 0; j < i; j++)
            {
                sql = sql + "and Time4 <> N'" + dayParser[j] + "' ";
            }
            for (int j = 0; j < i; j++)
            {
                sql = sql + "and Time5 <> N'" + dayParser[j] + "'";
            }
            for (int j = 0; j < i; j++)
            {
                sql = sql + "and Time6 <> N'" + dayParser[j] + "'";
            }
            for (int j = 0; j < i; j++)
            {
                sql = sql + "and Time7 <> N'" + dayParser[j] + "'";
            }
            for (int j = 0; j < i; j++)
            {
                sql = sql + "and Time8 <> N'" + dayParser[j] + "'";
            }
            for (int j = 0; j < k; j++)
            {
                sql = sql + "and className <> N'" + subjectParser[j] + "'";
            }
            sql = sql + "and ClassName Like N'%" + _subjectName + "%'";
            return this.db.Query<Subject>(sql).ToList();

        }
*/

    }
}
