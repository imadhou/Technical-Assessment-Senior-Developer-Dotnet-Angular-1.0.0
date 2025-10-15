using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2J.Application.Models.Exceptions
{
    public class UserAuthException: Exception
    {
        public UserAuthException(string message) : base(message) { }
    }
}
