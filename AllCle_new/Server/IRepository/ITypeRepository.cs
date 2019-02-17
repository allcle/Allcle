using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Models;

namespace Server.IRepository
{
    public interface ITypeRepository
    {
        List<Subject> GetSubjects();
        List<TypeClassNum> GetNormalCommon();
        List<TypeClassNum> GetNormalGeneral();
        List<TypeClassNum> GetEngAbeek();
        List<TypeClassNum> GetEngGeneral();
        List<TypeClassNum> GetArc();
        List<TypeClassNum> GetArcGeneral();
        List<TypeClassNum> GetMajor();
    }
}
