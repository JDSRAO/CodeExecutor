using System;
using System.Collections.Generic;
using System.Text;

namespace CodeExecutor.ConsoleTest.TestFiles
{
    public class Test01
    {
        public string HelloWorld(string anyString)
        {
            return anyString;
        }

        public static string HelloWorldStatic(string anyString)
        {
            return "kjds" + anyString;
        }

        public DateTime GetUtcDate()
        {
            return DateTime.UtcNow;
        }
    }
}
