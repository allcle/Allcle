using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 


namespace ApiTest.Models
{
    public interface ISubjectRepository
    {
        string AddSubject(Subject model);                                   // 입력 (로그인)
        List<Subject> GetSubjects();                                        // 전체 과목 출력
        List<Subject> GetSubjects(string _daylist);                         // 임의 과목을 선택했을 때, 해당 과목을 제외한 전체 과목 출력
        List<Subject> GetOnlySubjects(string _subjectName);                 // 입력된 subject name에 해당하는 정보만 출력
        List<Subject> GetSubjects(string _daylist, string _subjectName);    // 임의 과목을 선택한 뒤, 과목을 검색할 때, 임의 과목을 제외한 뒤, 검색 결과 출력
    }
}
