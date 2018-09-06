using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public interface ISubjectRepository2
    {
        //  string AddSubject(Subject model);                                   // 입력 (로그인)

        List<Subject> GetSubjects();
        

    }
}
