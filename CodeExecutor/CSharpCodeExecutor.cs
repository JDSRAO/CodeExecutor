using CT.CodeExecutor.Exceptions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace CT.CodeExecutor
{
    public class CSharpCodeExecutor
    {
        /// <summary>
        /// DLL file path
        /// </summary>
        public string DllPath { get; } = Path.GetTempPath();

        /// <summary>
        /// Libraries to be embedded into the .dll
        /// </summary>
        public List<string> AssemblyNames { get => assembliesToLoad; }

        /// <summary>
        /// Default class name with namespace
        /// </summary>
        public string ClassName { get; }
        
        /// <summary>
        /// Default method name without namespace
        /// </summary>
        public string MethodName { get; }

        private List<string> assembliesToLoad { get; set; }

        public CSharpCodeExecutor(string className, string methodToExecute, bool readAssembliesFromEnvironment = true, params string[] assemblies)
        {
            ClassName = className;
            MethodName = methodToExecute;
            assembliesToLoad = new List<string>();
            assembliesToLoad.AddRange(assemblies);
            if(readAssembliesFromEnvironment)
            {
                assembliesToLoad.AddRange(Environment.GetEnvironmentVariable("CodeExecutor:Assemblies").Split(';').ToList());
            }
        }

        /// <summary>
        /// Compiles and executes a c# code. Here the function to be executed will be picked from the constructor arguments
        /// </summary>
        /// <param name="code">C# code to be compiled and executed</param>
        /// <param name="arguments">Function arguments</param>
        /// <returns></returns>
        public object ExecuteCode(string code, params object[] arguments)
        {
            var dllPath = GenerateDll(code, assembliesToLoad);
            return ExecuteDll(ClassName, MethodName, dllPath, arguments);
        }

        /// <summary>
        /// Compiles and executes a c# code
        /// </summary>
        /// <param name="className">Name of the class with namespace</param>
        /// <param name="methodToExecute">Method to be executed</param>
        /// <param name="code">C# code to be compiled and executed</param>
        /// <param name="arguments">Function arguments</param>
        /// <returns></returns>
        public object ExecuteCode(string className, string methodToExecute, string code, params object[] arguments)
        {
            var dllPath = GenerateDll(code, assembliesToLoad);
            return ExecuteDll(className, methodToExecute, dllPath, arguments);
        }

        /// <summary>
        /// Compiles and executes a c# code
        /// </summary>
        /// <param name="className">Name of the class with namespace</param>
        /// <param name="methodToExecute">Method to be executed</param>
        /// <param name="code">C# code to be compiled and executed</param>
        /// <param name="assemblies">External assemblies to be added</param>
        /// <param name="arguments">Function arguments</param>
        /// <returns></returns>
        public object ExecuteCode(string className, string methodToExecute, string code, string[] assemblies, params object[] arguments)
        {
            var currentAssebliesToLoad = assembliesToLoad.ToList();
            currentAssebliesToLoad.AddRange(assemblies);
            var dllPath = GenerateDll(code, currentAssebliesToLoad);            
            return ExecuteDll(className, methodToExecute, dllPath, arguments);
        }

        /// <summary>
        /// Generates .dll file based on the code and assemblies provided
        /// </summary>
        /// <param name="code">C# code to be compiled and executed</param>
        /// <param name="assemblies">External assemblies to be added</param>
        /// <returns>Path where the .dll is created</returns>
        private string GenerateDll(string code, List<string> assemblies)
        {
            var tree = SyntaxFactory.ParseSyntaxTree(code);
            var parsedSyntaxTree = Parse(code, "", CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp6));
            string dllName = $"library.{DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")}.dll";

            // Detect the file location for the library that defines the object type
            //var references = GetReferences(typeof(Object), typeof(HttpClient), typeof(Task), typeof(WebRequest), typeof(Stream), typeof(MarshalByRefObject), typeof(Uri));
            var references = GetReferences(typeof(Object));

            var defaultCompilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithOverflowChecks(true)
                .WithOptimizationLevel(OptimizationLevel.Release);
            //.WithUsings(DefaultNamespaces);

            // A single, immutable invocation to the compiler
            // to produce a library
            var compilation = CSharpCompilation.Create(dllName)
                .WithOptions(defaultCompilationOptions)
                .AddReferences(references)
                .AddSyntaxTrees(parsedSyntaxTree);

            // Path where DLL is created
            string path = Path.Combine(DllPath, dllName);
            EmitResult compilationResult = compilation.Emit(path);
            if (compilationResult.Success)
            {
                return path;
            }
            else
            {
                var errrorMessage = new StringBuilder();
                foreach (Diagnostic codeIssue in compilationResult.Diagnostics)
                {
                    errrorMessage.AppendLine($"ID: {codeIssue.Id}, Message: {codeIssue.GetMessage()}, Location: { codeIssue.Location.GetLineSpan()}, Severity: { codeIssue.Severity}");
                }

                throw new CSharpCompilationException(errrorMessage.ToString());
            }
        }

        /// <summary>
        /// Executes a .dll file
        /// </summary>
        /// <param name="className">Name of the class with namespace</param>
        /// <param name="methodToExecute">Method to be executed</param>
        /// <param name="path">.dll path</param>
        /// <param name="arguments">Function arguments</param>
        /// <returns></returns>
        private object ExecuteDll(string className, string methodToExecute, string path, object[] arguments)
        {
            // Load the assembly
            Assembly asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
            // Invoke the method by passing an argument
            var classType = asm.GetType(className);
            var methodInfo = classType.GetMethod(methodToExecute);
            if(methodInfo != null)
            {
                object classInstance = null;
                if (!methodInfo.IsStatic)
                {
                    classInstance = Activator.CreateInstance(classType);
                }
                
                object result = methodInfo.Invoke(classInstance, arguments);
                return result;
            }
            else
            {
                throw new ArgumentNullException(methodToExecute);
            }
            
        }

        /// <summary>
        /// Parses the c# code
        /// </summary>
        /// <param name="text"></param>
        /// <param name="filename"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private SyntaxTree Parse(string text, string filename = "", CSharpParseOptions options = null)
        {
            var stringText = SourceText.From(text, Encoding.UTF8);
            return SyntaxFactory.ParseSyntaxTree(stringText, options, filename);
        }

        /// <summary>
        /// Gets library reference paths
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private PortableExecutableReference[] GetReferences(params Type[] items)
        {
            var references = new List<PortableExecutableReference>(items.Length);
            foreach (var item in items)
            {
                // Detect the file location for the library that defines the object type
                var refLocation = item.GetTypeInfo().Assembly.Location;
                // Create a reference to the library
                var reference = MetadataReference.CreateFromFile(refLocation);
                references.Add(reference);
            }

            foreach (var assemblyName in assembliesToLoad)
            {
                var runtimeassemblyLocation = Assembly.Load(assemblyName).Location;
                var runtimeassemblyReference = MetadataReference.CreateFromFile(runtimeassemblyLocation);
                references.Add(runtimeassemblyReference);
            }

            return references.ToArray();
        }
    }
}
