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
    }
}
