using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    class User
    {
        public string Id { get; set; }
        public string Password { get; set; }
        public string EncryptKey { get; set; }
        public string YearOfEntry { get; set; }
        public string College { get; set; }
        public string Major { get; set; }
    }
}
