using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Models;

namespace Server.IRepository
{
    public interface IMyGroupClassNumberRepository
    {
        List<MyGroupClassNumber> GetMyGroupClassNumbers(string _id, string _myGroupName);
    }
}
