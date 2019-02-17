using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.IRepository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Server.Controllers
{
    [Route("api/[controller]")]
    public class TypeController : Controller
    {
        private ITypeRepository _repo;   // ISubjectRepository class의 객체 _repo 생성.
        public TypeController(ITypeRepository repo) // private _repo에 데이터를 넣는 메소드.
        {
            _repo = repo;
        }
        // GET: api/<controller>
        [HttpGet]
        public List<Subject> GetSubjects()
        {
            return _repo.GetSubjects();
        }

        // GET api/<controller>/5
        [HttpGet("normal_common")]
        public List<TypeClassNum> GetNormalcommon()
        {
            return _repo.GetNormalCommon();
        }
        [HttpGet("normal_general")]
        public List<TypeClassNum> GetNormalGeneral()
        {
            return _repo.GetNormalGeneral();
        }

        [HttpGet("eng_abeek")]
        public List<TypeClassNum> GetEngAbeek()
        {
            return _repo.GetEngAbeek();
        }
        [HttpGet("eng_general")]
        public List<TypeClassNum> GetEngGeneral()
        {
            return _repo.GetEngGeneral();
        }

        [HttpGet("arc_")]
        public List<TypeClassNum> GetArc()
        {
            return _repo.GetArc();
        }
        [HttpGet("arc_general")]
        public List<TypeClassNum> GetArcGeneral()
        {
            return _repo.GetArcGeneral();
        }
        [HttpGet("major")]
        public List<TypeClassNum> GetMajor()
        {
            return _repo.GetMajor();
        }


    }
}
