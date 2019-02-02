using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Models;

namespace Server.IRepository
{
    public interface IMajorRepository
    {
        List<Major> GetMajor();
    }
}
