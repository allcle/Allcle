using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Models;

namespace Server.IRepository
{
    public interface IMyGroupClassNumberRepository
    {
        List<string> GetMyGroupClassNumbers(string _id, string MyGroupName);
        void PostSubjectToMyGroup(DelNameClassNum _myGroupClassNumber);
        bool GetClassNumber(string ID, string MyGroupName, string ClassNumber);
        void DeleteSubjectInMyGroup(DelNameClassNum myGroupClassNumber);
    }
}
