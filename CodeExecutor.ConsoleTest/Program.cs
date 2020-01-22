using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CT.CodeExecutor.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var defaultNameSpace = "CT.CodeExecutor.ConsoleTest.TestFiles";
            var tests = new List<CodeExecutorTests>()
            {
                new CodeExecutorTests { ClassName = $"{defaultNameSpace}.Test01", FileName = "Test01.cs", MethodName = "HelloWorld", Inputs = new object[] { "Hai" } },
                new CodeExecutorTests { ClassName = $"{defaultNameSpace}.Test01", FileName = "Test01.cs", MethodName = "HelloWorldStatic", Inputs = new object[] { "Hai" } },
                new CodeExecutorTests { ClassName = $"{defaultNameSpace}.Test01", FileName = "Test01.cs", MethodName = "GetUtcDate", Inputs = new object[] {  } },
                new CodeExecutorTests { ClassName = null, FileName = "JsTest01.js", MethodName = "ReturnString", Inputs = new object[] { "hello World" } },
                new CodeExecutorTests { ClassName = null, FileName = "JsTest01.js", MethodName = "getCurrentDate", Inputs = new object[] {  } }
            };
            Console.WriteLine("Started executing tests");
            try
            {
                var stopWatch = new Stopwatch();
                var cSharpcodeExecutor = new CSharpCodeExecutor("", "", false);
                var jsCodeExecutor = new JavaScriptCodeExecutor();
                var testOutput = new StringBuilder();
                foreach (var test in tests)
                {
                    Console.WriteLine($"Executing test : {test.FileName} , code type : {test.CodeType}");
                    testOutput.AppendLine($"Executing test file : {test.FileName} , method name : {test.MethodName}, code type : {test.CodeType}");
                    testOutput.AppendLine($"Test inputs : {JsonConvert.SerializeObject(test.Inputs)}");
                    if (test.CodeType == CodeType.CSharp)
                    {
                        stopWatch.Start();
                        dynamic output = cSharpcodeExecutor.ExecuteCode(test.ClassName, test.MethodName, test.Code, test.Inputs);
                        testOutput.AppendLine($"Test outputs : {JsonConvert.SerializeObject(output)}");
                        stopWatch.Stop();
                    }
                    else if(test.CodeType == CodeType.JS)
                    {
                        stopWatch.Start();
                        dynamic output = jsCodeExecutor.ExecuteCode(test.MethodName, test.Code, test.Inputs);
                        testOutput.AppendLine($"Test outputs : {JsonConvert.SerializeObject(output)}");
                        stopWatch.Stop();
                    }
                    var timeTakenMessage = $"Time taken for execution :  {stopWatch.Elapsed}";
                    Console.WriteLine(timeTakenMessage);
                    testOutput.AppendLine(timeTakenMessage);
                    Console.WriteLine($"Executed test : {test.FileName} , code type : {test.CodeType}");
                    testOutput.AppendLine($"Executed test file : {test.FileName} , method name : {test.MethodName}, code type : {test.CodeType}");
                    testOutput.AppendLine();
                    Console.WriteLine();
                }

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), DateTime.UtcNow.ToString("yyyyMMddHHmmssffff") + ".txt");
                File.WriteAllText(filePath, testOutput.ToString());
                Console.WriteLine($"Test output is generated at {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Executed tests!. Please press any key to exit.");
            Console.ReadKey();
        }
    }

    internal class CodeExecutorTests
    {
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public string FileName { get; set; }
        public object[] Inputs { get; set; }
        public string Code
        {
            get
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", FileName);
                return File.ReadAllText(path);
            }
        }
        public CodeType CodeType
        {
            get
            {
                if(FileName.EndsWith(".cs"))
                {
                    return CodeType.CSharp;
                }
                else if(FileName.EndsWith(".js"))
                {
                    return CodeType.JS;
                }
                else
                {
                    throw new Exception("Unknown code type");
                }
            }
        }

        public CodeExecutorTests()
        {
            Inputs = new object[] { }; //new List<object>().ToArray();
        }
    }

    internal enum CodeType
    {
        CSharp,
        JS
    }
}
