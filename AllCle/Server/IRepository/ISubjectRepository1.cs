using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Models;

namespace Server.IRepository
{
    public interface ISubjectRepository1
    {
         List<Subject> GetSubjects();                             //전체 모든 과목 보기
    }
}
