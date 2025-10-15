using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2J.Application.Models.Exceptions
{
    public class ReferenceNotFoundException: Exception
    {
        public ReferenceNotFoundException(string message) : base(message) { }
    }
}
