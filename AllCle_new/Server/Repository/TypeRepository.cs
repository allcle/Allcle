using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Server.Models;
using Dapper;
using Server.IRepository;

namespace Server.Repository
{
    public class TypeRepository : ITypeRepository
    {

        private string Sub = null;
        private string NormalCommon = null;
        private string NormalGeneral = null;
        private string EngAbeek = null;
        private string EngGeneral = null;
        private string Arc = null;
        private string ArcGeneral = null;
        private string Major = null;

        private IConfiguration _config;
        private SqlConnection db;
        public TypeRepository(IConfiguration config)                                             // db 설정하는 메소드
        {
            _config = config;

            // IConfiguration 개체를 통해서 
            // appsettings.json의 데이터베이스 연결 문자열을 읽어온다. 
            db = new SqlConnection(
                _config.GetSection("ConnectionStrings").GetSection(
                    "DefaultConnection").Value);
            List<string> vs = new List<string>();
            string sql = "SELECT * FROM TableName";
            vs = this.db.Query<string>(sql).ToList();
            Sub = vs.Where(s => s.Contains("Sub")).ToList()[0];
            NormalCommon = vs.Where(s => s.Contains("normal_common")).ToList()[0];
            NormalGeneral = vs.Where(s => s.Contains("normal_general")).ToList()[0];
            EngAbeek = vs.Where(s => s.Contains("eng_abeek")).ToList()[0];
            EngGeneral = vs.Where(s => s.Contains("eng_general")).ToList()[0];
            Arc = vs.Where(s => s.Contains("arc_")).ToList()[0];
            ArcGeneral = vs.Where(s => s.Contains("arc_general")).ToList()[0];
            Major = vs.Where(s => s.Contains("major")).ToList()[0];
        }
        public List<Subject> GetSubjects()
        {
            string sql = "SELECT * FROM " + Sub;
            return this.db.Query<Subject>(sql).ToList();
        }

        public List<TypeClassNum> GetNormalCommon()
        {
            string sql = "SELECT * FROM " +  NormalCommon ;
            return this.db.Query<TypeClassNum>(sql).ToList();
        }
        public List<TypeClassNum> GetNormalGeneral()
        {
            string sql = "SELECT * FROM " + NormalGeneral;
            return this.db.Query<TypeClassNum>(sql).ToList();
        }
        public List<TypeClassNum> GetEngAbeek()
        {
            string sql = "SELECT * FROM " + EngAbeek;
            return this.db.Query<TypeClassNum>(sql).ToList();
        }
        public List<TypeClassNum> GetEngGeneral()
        {
            string sql = "SELECT * FROM " + EngGeneral;
            return this.db.Query<TypeClassNum>(sql).ToList();
        }
        public List<TypeClassNum> GetArc()
        {
            string sql = "SELECT * FROM " + Arc;
            return this.db.Query<TypeClassNum>(sql).ToList();
        }
        public List<TypeClassNum> GetArcGeneral()
        {
            string sql = "SELECT * FROM " + ArcGeneral ;
            return this.db.Query<TypeClassNum>(sql).ToList();
        }
        public List<TypeClassNum> GetMajor()
        {
            string sql = "SELECT * FROM "+ Major;
            return this.db.Query<TypeClassNum>(sql).ToList();
        }
    }
}
