using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLox
{
    public class CsLoxParseException : Exception
    {
        public CsLoxParseException(string? message) : base(message)
        {
        }
    }
}
