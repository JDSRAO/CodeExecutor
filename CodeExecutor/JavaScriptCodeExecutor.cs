using Jint;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeExecutor
{
    public class JavaScriptCodeExecutor
    {
        /// <summary>
        /// Executes a JS code outside of a function
        /// </summary>
        /// <param name="code">Script code</param>
        /// <param name="arguments">Function arguments</param>
        /// <returns></returns>
        public object ExecuteCode(string code, params object[] arguments)
        {
            var output = new Engine()
                      .Execute("function CodeExecutor(inputValue) { " + code + " }")
                    .Invoke("CodeExecutor", arguments)
                      .ToObject();
            return output;
        }

        /// <summary>
        /// Executes a JS script 
        /// </summary>
        /// <param name="functionToExecute">Name of the function to be executed</param>
        /// <param name="code">Script code</param>
        /// <param name="arguments">Function arguments</param>
        /// <returns></returns>
        public object ExecuteCode(string functionToExecute, string code, params object[] arguments)
        {
            var output = new Engine()
                      .Execute(code)
                    .Invoke(functionToExecute, arguments)
                      .ToObject();
            return output;
        }
    }
}
