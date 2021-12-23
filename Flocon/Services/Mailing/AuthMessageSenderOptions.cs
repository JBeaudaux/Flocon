using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flocon.Services.Mailing
{
    public class AuthMessageSenderOptions
    {
        public string SendGridUser { get; set; }
        public string SendGridKey { get; set; }
    }
}
