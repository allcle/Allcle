using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace Server.Models
{
    public class UserRepository : IUserRepository
    {
        private IConfiguration _config;
        private SqlConnection db;
        public UserRepository(IConfiguration config)                                             // db 설정하는 메소드
        {
            _config = config;

            // IConfiguration 개체를 통해서 
            // appsettings.json의 데이터베이스 연결 문자열을 읽어온다. 
            db = new SqlConnection(
                _config.GetSection("ConnectionStrings").GetSection(
                    "DefaultConnection").Value);
        }
        public List<User> GetUsers()                             //전체 모든 과목 보기
        {
            string sql = "Select * From Users Order by NO Asc";
            return this.db.Query<User>(sql).ToList();
        }

        public string Decrypt(string strEncrypted, string strKey)
        {
            TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();
            byte[] byteHash, byteBuff;
            string strTempKey = strKey;
            byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
            objHashMD5 = null;
            objDESCrypto.Key = byteHash;
            objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB
            byteBuff = Convert.FromBase64String(strEncrypted);
            string strDecrypted = ASCIIEncoding.ASCII.GetString
            (objDESCrypto.CreateDecryptor().TransformFinalBlock
            (byteBuff, 0, byteBuff.Length));
            objDESCrypto = null;
            return strDecrypted;
        }

        public void PostUsers(User _user)
        {
            string setkey = "allcle";
            _user.Password = Decrypt(_user.Password, setkey);
            string sql = "Insert Into Users (Id, Password) Values (@Id, @Password)";
            db.Execute(sql, _user);
        }



    }
}
