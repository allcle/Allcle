using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public interface ISubjectRepository1
    {
        /*string AddSubject(Subject model);                                   // 입력 (로그인)
        List<Subject> GetSubjects();                                        // 전체 과목 출력
        List<Subject> GetSubjects(string _daylist);                         // 임의 과목을 선택했을 때, 해당 과목을 제외한 전체 과목 출력
        List<Subject> GetOnlySubjects(string _subjectName);                 // 입력된 subject name에 해당하는 정보만 출력
        List<Subject> GetSubjects(string _daylist, string _subjectName);    // 임의 과목을 선택한 뒤, 과목을 검색할 때, 임의 과목을 제외한 뒤, 검색 결과 출력
    */

        List<Subject> GetTimeOnSubjectOnSearchOff(string _time, string _subject);  //남은시간에서만, 담은과목 제외
        List<Subject> GetTimeOnSubjectOffSearchOff(string _subject);                //담은 과목만 제외
        List<Subject> GetTimeOffSubjectOnSearchOff(string _time);                 //남은 시간만에서만
        List<Subject> GetTimeOffSubjectOffSearchOff();                             //전체 모든 과목 보기

        List<Subject> GetTimeOnSubjectOnSearchOn(string _time, string _subject, string _search); //남은시간에서만, 담은과목제외, 검색
        List<Subject> GetTimeOffSubjectOnSearchOn(string _subject, string _search);              //담은 과목 제외하고 검색
        List<Subject> GetTimeOnSubjectOffSearchOn(string _time, string _search);                 //남은 시간에서만 검색
        List<Subject> GetTimeOffSubjectOffSearchOn(string _search);                              //그냥 과목검색
    }
}
