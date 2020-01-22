using System;
using System.Collections.Generic;
using System.Text;

namespace CT.CodeExecutor.Exceptions
{
    public class CSharpCompilationException : Exception
    {
        public CSharpCompilationException(string message) : base(message)
        {

        }
    }
}
