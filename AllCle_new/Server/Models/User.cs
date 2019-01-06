using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Password { get; set; }
        public string EncryptKey { get; set; }
        public string YearOfEntry { get; set; }
        public string College { get; set; }
        public string Major { get; set; }
    }
}
