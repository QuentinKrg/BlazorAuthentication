using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWasmAuthentication.Shared
{
    public class User
    {
        public string UserLogin { get; set; }
        public string Password { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
       
    }
}
