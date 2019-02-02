using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Models;

namespace Server.IRepository
{
    public interface IEngNormalRepository
    {
        List<EngNormal> GetEngNormal();
    }
}
