using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceApp
{
    public class TokenRequestModel
    {
        public string grant_type { get; set; } = "password";
        public string username { get; set; }
        public string password { get; set; }

        public override string ToString() => $"grant_type={grant_type}&username={username}&password={password}";
    }
}
